using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Grid-based A* pathfinding bot for 2D. 
/// - Builds a local grid (walkable / blocked by obstacles layer)
/// - Runs A* to compute a path to the player
/// - Follows the path using Rigidbody2D.MovePosition
/// - Attacks the player (calls PlayerHealth.TakeDamage) when in attackRange with cooldown
/// 
/// Usage:
/// - Attach to enemy GameObject with Rigidbody2D
/// - Set the obstacleLayer to whatever layer(s) are blocking movement (Tilemap collider, walls, etc.)
/// - Tag the Player GameObject as "Player" and give it a PlayerHealth component
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class BotFinder : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 2.5f;
    [Tooltip("Minimum distance to consider we've reached a path node")]
    public float nodeReachThreshold = 0.05f;

    [Header("Attack")]
    public float attackRange = 0.9f;
    public int attackDamage = 10;
    public float attackCooldown = 1.0f;

    [Header("Chase")]
    [Tooltip("If > 0, bot stops chasing if player is farther than this")]
    public float maxChaseDistance = 0f;

    [Header("Pathfinding Grid")]
    [Tooltip("World size of the grid centered on the bot used for pathfinding")]
    public Vector2 gridWorldSize = new Vector2(12f, 12f);
    [Tooltip("Radius of a single node (half node size)")]
    public float nodeRadius = 0.25f;
    [Tooltip("Layer(s) considered obstacles for the grid")]
    public LayerMask obstacleLayer;

    [Header("Repathing")]
    [Tooltip("How often (seconds) the bot recalculates the path")]
    public float pathRecalcInterval = 0.5f;
    [Tooltip("Minimum player movement (world units) to trigger new path immediately")]
    public float playerMoveThreshold = 0.5f;

    // runtime
    private Transform playerTransform;
    private PlayerHealth playerHealth;
    private Rigidbody2D rb;
    private float attackTimer = 0f;

    // grid
    private Node[,] grid;
    private int gridSizeX, gridSizeY;
    private float nodeDiameter;

    // path
    private List<Vector2> path = new List<Vector2>();
    private int currentPathIndex = 0;
    private Vector2 lastPlayerPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        FindPlayer();
        nodeDiameter = nodeRadius * 2f;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        StartCoroutine(PathRoutine());
    }

    void Update()
    {
        if (playerTransform == null) FindPlayer();

        if (attackTimer > 0f) attackTimer -= Time.deltaTime;
    }

    void FixedUpdate()
    {
        if (playerTransform == null) return;

        // Optional chase distance limit
        if (maxChaseDistance > 0f)
        {
            if (Vector2.Distance(transform.position, playerTransform.position) > maxChaseDistance) return;
        }

        // Follow path if exists
        if (path != null && path.Count > 0 && currentPathIndex < path.Count)
        {
            Vector2 targetPos = path[currentPathIndex];
            Vector2 myPos = rb.position;
            Vector2 newPos = Vector2.MoveTowards(myPos, targetPos, moveSpeed * Time.fixedDeltaTime);
            rb.MovePosition(newPos);

            if (Vector2.Distance(newPos, targetPos) <= nodeReachThreshold)
            {
                currentPathIndex = Mathf.Min(currentPathIndex + 1, path.Count);
            }
        }
        else
        {
            // If no path, try to move directly if outside attack range
            float dist = Vector2.Distance(rb.position, playerTransform.position);
            if (dist > attackRange)
            {
                Vector2 dir = (playerTransform.position - transform.position).normalized;
                Vector2 newPos = rb.position + dir * (moveSpeed * Time.fixedDeltaTime);
                rb.MovePosition(newPos);
            }
        }

        // If within attack range, attack
        float playerDist = Vector2.Distance(transform.position, playerTransform.position);
        if (playerDist <= attackRange)
        {
            TryAttack();
        }
    }

    private void TryAttack()
    {
        if (playerHealth == null) return;

        if (attackTimer <= 0f)
        {
            playerHealth.TakeDamage(attackDamage);
            attackTimer = attackCooldown;
        }
    }

    private IEnumerator PathRoutine()
    {
        // Build grid once, then recalc path periodically
        BuildGrid();
        lastPlayerPosition = playerTransform != null ? (Vector2)playerTransform.position : Vector2.zero;

        while (true)
        {
            if (playerTransform != null)
            {
                // Rebuild grid around current bot position (useful if obstacles are dynamic or bot moved significantly)
                BuildGrid();

                // Recalculate path if player moved sufficiently or at interval
                if (Vector2.Distance(lastPlayerPosition, playerTransform.position) >= playerMoveThreshold || path.Count == 0)
                {
                    CalculatePathToPlayer();
                    lastPlayerPosition = playerTransform.position;
                }
            }

            yield return new WaitForSeconds(pathRecalcInterval);
        }
    }

    private void BuildGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector2 worldBottomLeft = (Vector2)transform.position - new Vector2(gridWorldSize.x / 2f, gridWorldSize.y / 2f);

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector2 worldPoint = worldBottomLeft + new Vector2(x * nodeDiameter + nodeRadius, y * nodeDiameter + nodeRadius);
                bool walkable = !Physics2D.OverlapCircle(worldPoint, nodeRadius * 0.9f, obstacleLayer);
                grid[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }
    }

    private void CalculatePathToPlayer()
    {
        path.Clear();
        currentPathIndex = 0;
        if (playerTransform == null) return;

        Node startNode = NodeFromWorldPoint(transform.position);
        Node targetNode = NodeFromWorldPoint(playerTransform.position);
        if (startNode == null || targetNode == null || !targetNode.walkable)
        {
            // fallback: no valid path, just clear
            return;
        }

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost))
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                RetracePath(startNode, targetNode);
                return;
            }

            foreach (Node neighbour in GetNeighbours(currentNode))
            {
                if (!neighbour.walkable || closedSet.Contains(neighbour)) continue;

                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }
    }

    private void RetracePath(Node startNode, Node endNode)
    {
        List<Node> nodes = new List<Node>();
        Node current = endNode;
        while (current != startNode)
        {
            nodes.Add(current);
            if (current.parent == null) break; // safety
            current = current.parent;
        }
        nodes.Reverse();

        path.Clear();
        foreach (Node n in nodes)
        {
            path.Add(n.worldPosition);
        }

        currentPathIndex = 0;
    }

    private Node NodeFromWorldPoint(Vector2 worldPosition)
    {
        if (grid == null) return null;

        Vector2 worldBottomLeft = (Vector2)transform.position - new Vector2(gridWorldSize.x / 2f, gridWorldSize.y / 2f);
        float percentX = (worldPosition.x - worldBottomLeft.x) / (gridWorldSize.x);
        float percentY = (worldPosition.y - worldBottomLeft.y) / (gridWorldSize.y);
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.Clamp(Mathf.RoundToInt((gridSizeX - 1) * percentX), 0, gridSizeX - 1);
        int y = Mathf.Clamp(Mathf.RoundToInt((gridSizeY - 1) * percentY), 0, gridSizeY - 1);
        return grid[x, y];
    }

    private List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;
                // optionally prevent diagonal movement through corners:
                if (Mathf.Abs(x) == 1 && Mathf.Abs(y) == 1)
                {
                    // allow diagonals but only if both adjacent cardinal nodes are walkable (prevents cutting corners)
                    Node n1 = GetNode(node.gridX + x, node.gridY);
                    Node n2 = GetNode(node.gridX, node.gridY + y);
                    if (n1 == null || n2 == null || !n1.walkable || !n2.walkable) continue;
                }

                Node neighbour = GetNode(node.gridX + x, node.gridY + y);
                if (neighbour != null) neighbours.Add(neighbour);
            }
        }

        return neighbours;
    }

    private Node GetNode(int x, int y)
    {
        if (x >= 0 && x < gridSizeX && y >= 0 && y < gridSizeY) return grid[x, y];
        return null;
    }

    private int GetDistance(Node a, Node b)
    {
        int dstX = Mathf.Abs(a.gridX - b.gridX);
        int dstY = Mathf.Abs(a.gridY - b.gridY);
        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }

    private void FindPlayer()
    {
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
            playerHealth = playerObj.GetComponent<PlayerHealth>();
            lastPlayerPosition = playerTransform.position;
        }
        else
        {
            var pc = FindObjectOfType<PlayerController2D>();
            if (pc != null)
            {
                playerTransform = pc.transform;
                playerHealth = pc.GetComponent<PlayerHealth>();
                lastPlayerPosition = playerTransform.position;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        // visualize grid & path
        Gizmos.color = Color.gray;
        if (grid != null)
        {
            foreach (Node n in grid)
            {
                Gizmos.color = n.walkable ? new Color(0.3f, 0.7f, 0.3f, 0.25f) : new Color(0.7f, 0.2f, 0.2f, 0.6f);
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter * 0.9f));
            }
        }

        if (path != null && path.Count > 0)
        {
            Gizmos.color = Color.cyan;
            for (int i = 0; i < path.Count - 1; i++)
            {
                Gizmos.DrawLine(path[i], path[i + 1]);
                Gizmos.DrawSphere(path[i], 0.05f);
            }
            Gizmos.DrawSphere(path[path.Count - 1], 0.05f);
        }

        // attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    // Node class used by A*
    private class Node
    {
        public bool walkable;
        public Vector2 worldPosition;
        public int gridX;
        public int gridY;

        public int gCost = int.MaxValue;
        public int hCost;
        public Node parent;

        public Node(bool walkable, Vector2 worldPos, int x, int y)
        {
            this.walkable = walkable;
            worldPosition = worldPos;
            gridX = x;
            gridY = y;
        }

        public int fCost => (gCost == int.MaxValue ? int.MaxValue : gCost + hCost);
    }
}