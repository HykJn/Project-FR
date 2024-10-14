using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TileNode
{
    public TileNode parent;
    public int x, y;
    public bool isWall;

    public int G, H;
    public int F
    {
        get
        {
            return G + H;
        }
    }

    public TileNode(int x, int y, bool isWall)
    {
        this.x = x;
        this.y = y;
        this.isWall = isWall;
    }
}

public abstract class Enemy : MonoBehaviour
{
    public enum State
    {
        Idle, Chase, Attack, Hit, None
    }

    [Header("-- General")]
    public State curState = State.Idle;
    public float _fMaxHP, _fCurHP, _fBaseSpeed, _fAttackSpeed;
    public int _iDistanceToChase, _iDistanceToAttack;

    protected float _fDistanceToPlayer, _tAttackSpeed;
    protected Animator animator;

    [Header("-- Pathfinding")]
    public TileNode[,] nodeMap;
    public List<TileNode> openList;
    public List<TileNode> closeList;
    public List<TileNode> pathList;
    public TileNode startNode, targetNode, curNode;
    public Vector2Int targetPos, startPos, bottomLeft, topRight;

    protected virtual void Start()
    {
        animator = this.GetComponent<Animator>();
    }

    protected virtual void Update()
    {
        _fDistanceToPlayer = Vector2.Distance(this.transform.position, GameObject.FindWithTag("Player").transform.position);
        ChangeState(curState);
        //Pathfinding();
    }

    public void ChangeState(State state)
    {
        switch (state)
        {
            case State.Idle:
                Idle();
                break;
            case State.Chase:
                Chase();
                break;
            case State.Attack:
                Attack();
                break;
        }
    }

    protected abstract void Idle();
    protected abstract void Chase();
    protected abstract void Attack();
    protected abstract void Hit(float damage);

    protected void Pathfinding()
    {
        //Init variable
        startPos = Vector2Int.RoundToInt(this.transform.position);
        targetPos = Vector2Int.RoundToInt(GameObject.FindWithTag("Player").transform.position);

        bottomLeft = new Vector2Int(startPos.x - _iDistanceToChase, startPos.y - _iDistanceToChase);
        topRight = new Vector2Int(startPos.x + _iDistanceToChase, startPos.y + _iDistanceToChase);

        int idx_x = topRight.x - bottomLeft.x + 1;
        int idx_y = topRight.y - bottomLeft.y + 1;
        nodeMap = new TileNode[idx_x, idx_y];

        //Get node info
        for (int x = 0; x < idx_x; x++)
        {
            for (int y = 0; y < idx_y; y++)
            {
                Vector2 point = new Vector2Int(bottomLeft.x + x, bottomLeft.y + y);
                RaycastHit2D hit = Physics2D.Raycast(point, Vector3.forward, 0f, LayerMask.GetMask("Wall"));

                if (hit)
                {
                    nodeMap[x, y] = new TileNode((int)point.x, (int)point.y, true);
                    Debug.DrawRay(point, Vector3.forward, Color.red);
                }
                else
                {
                    nodeMap[x, y] = new TileNode((int)point.x, (int)point.y, false);
                }
            }
        }

        //Init Nodes
        startNode = nodeMap[startPos.x - bottomLeft.x, startPos.y - bottomLeft.y];
        targetNode = nodeMap[targetPos.x - bottomLeft.x, targetPos.y - bottomLeft.y];
        openList = new List<TileNode>(200) { startNode };
        closeList = new List<TileNode>(200);
        pathList = new List<TileNode>(200);

        //A* algorithm
        while (openList.Count > 0)
        {
            //Get node that has smallest F value from openList
            //When nodes have same F then compare H value
            curNode = openList[0];
            foreach (TileNode node in openList)
            {
                if (node.F < curNode.F) curNode = node;
                else if (node.F == curNode.F && node.H < curNode.H) curNode = node;
            }
            openList.Remove(curNode);
            closeList.Add(curNode);

            //When find path
            if (curNode == targetNode)
            {
                TileNode temp = curNode;
                while (temp != null)
                {
                    pathList.Add(temp);
                    temp = temp.parent;
                }
                pathList.Reverse();
                break;
            }

            //Expansion each 8 direction
            Expansion(curNode.x + 1, curNode.y + 1);    //¢Ö
            Expansion(curNode.x + 1, curNode.y);        //¡æ
            Expansion(curNode.x + 1, curNode.y - 1);    //¢Ù
            Expansion(curNode.x, curNode.y + 1);        //¡è
            Expansion(curNode.x, curNode.y - 1);        //¡é
            Expansion(curNode.x - 1, curNode.y + 1);    //¢Ø
            Expansion(curNode.x - 1, curNode.y);        //¡ç
            Expansion(curNode.x - 1, curNode.y - 1);    //¢×
        }

        if (pathList.Count > 0)
        {
            for (int i = 0; i < pathList.Count - 1; i++)
            {
                Vector2 from = new Vector2(pathList[i].x, pathList[i].y);
                Vector2 to = new Vector2(pathList[i + 1].x, pathList[i + 1].y);
                Debug.DrawLine(from, to, Color.red);
            }
        }
    }

    protected void Expansion(int childX, int childY)
    {
        bool inBoundary = (childX <= topRight.x && childX >= bottomLeft.x) && (childY <= topRight.y && childY >= bottomLeft.y);
        if (inBoundary)
        {
            TileNode childNode = nodeMap[childX - bottomLeft.x, childY - bottomLeft.y];
            bool isWall = childNode.isWall;
            bool inCloseList = closeList.Contains(childNode);
            bool isCorner = nodeMap[childX - bottomLeft.x, curNode.y - bottomLeft.y].isWall || nodeMap[curNode.x - bottomLeft.x, childY - bottomLeft.y].isWall;

            if (!isWall && !inCloseList && !isCorner)
            {
                int arc = curNode.G + (childX == curNode.x || childY == curNode.y ? 10 : 14);
                if (arc < childNode.G || !openList.Contains(childNode))
                {
                    childNode.G = arc;
                    childNode.H = (Mathf.Abs(targetNode.x - childX) + Mathf.Abs(targetNode.y - childY)) * 10;
                    childNode.parent = curNode;
                    openList.Add(childNode);
                }
            }
        }
    }

    protected void OnDrawGizmos()
    {
        Gizmos.DrawRay(this.transform.position, new Vector2(0, 1) * _iDistanceToChase);
        Gizmos.DrawRay(this.transform.position, new Vector2(0, -1) * _iDistanceToChase);
        Gizmos.DrawRay(this.transform.position, new Vector2(1, 0) * _iDistanceToChase);
        Gizmos.DrawRay(this.transform.position, new Vector2(-1, 0) * _iDistanceToChase);

        Gizmos.DrawLine(new Vector2(this.transform.position.x - _iDistanceToChase, this.transform.position.y - _iDistanceToChase), new Vector2(this.transform.position.x + _iDistanceToChase, this.transform.position.y - _iDistanceToChase));
        Gizmos.DrawLine(new Vector2(this.transform.position.x - _iDistanceToChase, this.transform.position.y + _iDistanceToChase), new Vector2(this.transform.position.x + _iDistanceToChase, this.transform.position.y + _iDistanceToChase));
        Gizmos.DrawLine(new Vector2(this.transform.position.x - _iDistanceToChase, this.transform.position.y - _iDistanceToChase), new Vector2(this.transform.position.x - _iDistanceToChase, this.transform.position.y + _iDistanceToChase));
        Gizmos.DrawLine(new Vector2(this.transform.position.x + _iDistanceToChase, this.transform.position.y - _iDistanceToChase), new Vector2(this.transform.position.x + _iDistanceToChase, this.transform.position.y + _iDistanceToChase));
    }
}
