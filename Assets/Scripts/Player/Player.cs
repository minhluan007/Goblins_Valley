using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour
{
    [Header("Player Info")]
    [SerializeField]
    private float walkSpeed = 0.5f;
    [SerializeField]
    private float runSpeed = 0.8f;

    Animator myAnimator;
    SpriteRenderer mySpriteRenderer;
    Rigidbody2D myRigidbody2D;
    private bool playerMove;

    [Header("Tile Map")]
    public Tilemap map;
    public RuleTile[] walkableTiles;


    public enum TypeMove
    {
        Walk,
        Run
    }

    public TypeMove currentMove;

    private void Start()
    {
        myAnimator = GetComponent<Animator>();
        mySpriteRenderer = GetComponent<SpriteRenderer>();
        myRigidbody2D = GetComponent<Rigidbody2D>();

        playerMove = true;
        currentMove = TypeMove.Walk;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            currentMove = TypeMove.Run;
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            currentMove = TypeMove.Walk;
        }
    }

    private void FixedUpdate()
    {
        if (playerMove)
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");
            float speed = (currentMove == TypeMove.Walk) ? walkSpeed : runSpeed;

            Vector3Int currentCell = map.WorldToCell(transform.position);

            if (Input.GetKeyDown(KeyCode.W))
                TryMove(currentCell + Vector3Int.up);
            else if (Input.GetKeyDown(KeyCode.A))
                TryMove(currentCell + Vector3Int.left);
            else if (Input.GetKeyDown(KeyCode.S))
                TryMove(currentCell + Vector3Int.down);
            else if (Input.GetKeyDown(KeyCode.D))
                TryMove(currentCell + Vector3Int.right);

            if (Mathf.Abs(horizontalInput) > 0 && Mathf.Abs(verticalInput) > 0)
            {
                horizontalInput *= 0.7071f;
                verticalInput *= 0.7071f;
            }
            if (horizontalInput < 0)
            {
                mySpriteRenderer.flipX = true;
            }
            else if (horizontalInput > 0)
            {
                mySpriteRenderer.flipX = false;
            }

            Vector3 movement = new Vector3(horizontalInput, verticalInput, 0f) * speed;
            myRigidbody2D.velocity = movement;

            if (currentMove == TypeMove.Run && movement.magnitude > 0)
            {
                PlayerAnimatorIdle(false);
                PlayerAnimatorRun(true);
                PlayerAnimatorWalk(false);
            }
            else if (currentMove == TypeMove.Walk && movement.magnitude > 0)
            {
                PlayerAnimatorIdle(false);
                PlayerAnimatorRun(false);
                PlayerAnimatorWalk(true);
            }
            else
            {
                PlayerAnimatorRun(false);
                PlayerAnimatorWalk(false);
                PlayerAnimatorIdle(true);
            }
        }

        if (currentMove == TypeMove.Run)
        {
            PlayerAnimatorRun(true);
        }
        else
        {
            PlayerAnimatorRun(false);
        }
    }

    private void PlayerAnimatorRun(bool isBool)
    {
        myAnimator.SetBool("Run", isBool);
    }

    private void PlayerAnimatorWalk(bool isBool)
    {
        myAnimator.SetBool("Walk", isBool);
    }

    private void PlayerAnimatorIdle(bool isBool)
    {
        myAnimator.SetBool("Idle", isBool);
    }

    private void TryMove(Vector3Int targetCell)
    {
        TileBase tile = map.GetTile(targetCell);

        if (IsTileWalkable(tile))
        {
            transform.position = map.GetCellCenterWorld(targetCell);
        }
    }

    private bool IsTileWalkable(TileBase tile)
    {
        foreach (RuleTile walkableTile in walkableTiles)
        {
            if (walkableTile != null && walkableTile.Equals(tile))
                return true;
        }

        return false;
    }
}