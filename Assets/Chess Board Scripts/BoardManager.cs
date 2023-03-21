using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public float tileSize;

    public ChessFigure[,] figurePositions;

    public GameObject[] pieces;
    public List<GameObject> active;

    public int powerR;
    public int powerC;

    private bool isWhiteTurn;
    private ChessFigure selected;
    private bool[,] legalMoves;

    public int selectionX;
    public int selectionY;

    private static BoardManager _instance;

    public static BoardManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<BoardManager>();
            }

            return _instance;
        }
    }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        figurePositions = new ChessFigure[8, 8];
        powerR = -1;
        powerC = -1;
        active = new List<GameObject>();
        isWhiteTurn = true;

        SpawnAllChessFigures();
    }

    void Update()
    {
        DrawChessBoard();
        UpdateSelection();

        if (Input.GetMouseButtonDown(0))
        {
            if (selectionX >= 0 && selectionY >= 0)
            {
                if (selected == null) SelectChessFigure(selectionX, selectionY);
                else MoveChessFigure(selectionX, selectionY);
            }
        }
    }

    private void UpdateSelection()
    {
        RaycastHit hit;
        float raycastDistance = 100.0f;
        Debug.Log("in");
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, raycastDistance))
        {
            Debug.Log("hit");
            selectionX = (int) hit.point.x;
            selectionY = (int) hit.point.z;
        }
        else
        {
            selectionX = -1;
            selectionY = -1;
        }
    }

    private void DrawChessBoard()
    {
        Vector3 widthLine = Vector3.right * 8;
        Vector3 heightLine = Vector3.forward * 8;

        for (int i = 0; i <= 8; i++)
        {
            Vector3 start = Vector3.forward * i;
            Debug.DrawLine(start, start + widthLine);
            for (int j = 0; j <= 8; j++)
            {
                start = Vector3.right * j;
                Debug.DrawLine(start, start + heightLine);
            }
        }

        if (selectionX >= 0 && selectionY >= 0)
        {
            Debug.DrawLine(Vector3.forward * selectionY + Vector3.right * selectionX,
                Vector3.forward * (selectionY + 1) + Vector3.right * (selectionX + 1));
            Debug.DrawLine(Vector3.forward * selectionY + Vector3.right * (selectionX + 1),
                Vector3.forward * (selectionY + 1) + Vector3.right * selectionX);
        }
    }

    private void SpawnAllChessFigures()
    {
        //white pieces
        SpawnChessFigure(6, 0, 3); //king
        SpawnChessFigure(7, 0, 4); //queen
        SpawnChessFigure(8, 0, 0); SpawnChessFigure(8, 0, 7); //rooks
        SpawnChessFigure(9, 0, 2); SpawnChessFigure(9, 0, 5); //bishops
        SpawnChessFigure(10, 0, 1); SpawnChessFigure(10, 0, 6); //knights
        for (int i = 0; i < 8; i++)
        {
            SpawnChessFigure(11, 1, i); //pawn
        }

        //black pieces
        SpawnChessFigure(0, 7, 3); //king
        SpawnChessFigure(1, 7, 4); //queen
        SpawnChessFigure(2, 7, 0); SpawnChessFigure(2, 7, 7); //rooks
        SpawnChessFigure(3, 7, 2); SpawnChessFigure(3, 7, 5); //bishops
        SpawnChessFigure(4, 7, 1); SpawnChessFigure(4, 7, 6); //knights
        for (int i = 0; i < 8; i++)
        {
            SpawnChessFigure(5, 6, i); //pawn
        }
    }

    private void SpawnChessFigure(int index, int r, int c)
    {
        GameObject piece = Instantiate(pieces[index], GetTileCenter(r, c), pieces[index].transform.rotation) as GameObject;
        piece.transform.SetParent(transform);
        figurePositions[r, c] = piece.GetComponent<ChessFigure>();
        figurePositions[r, c].SetPosition(r, c);
        active.Add(piece);
    }

    private Vector3 GetTileCenter(int r, int c)
    {
        Vector3 origin = Vector3.zero;
        origin.z += (tileSize * r) + tileSize / 2;
        origin.x += (tileSize * c) + tileSize / 2;
        return origin;
    }

    private void SelectChessFigure(int r, int c)
    {
        if (figurePositions[r, c] == null) return;
        if (figurePositions[r, c].isWhite != isWhiteTurn) return;

        bool hasMove = false;
        legalMoves = figurePositions[r, c].PossibleMove();

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (legalMoves[i, j])
                {
                    hasMove = true;
                    i = 7;
                    break;
                }
            }
        }
        if (!hasMove)
        {
            selected = null;
            return;
        }

        selected = figurePositions[r, c];
        //highlight legal moves
    }

    private void MoveChessFigure(int r, int c)
    {
        if (legalMoves[r, c])
        {
            ChessFigure square = figurePositions[r, c];
            if (square != null && square.isWhite != isWhiteTurn) //capture piece
            {
                active.Remove(square.gameObject);
                Destroy(square.gameObject);
            }

            figurePositions[selected.CurrentR, selected.CurrentC] = null;
            selected.transform.position = GetTileCenter(r, c);
            selected.SetPosition(r, c);
            figurePositions[r, c] = selected;
            isWhiteTurn = !isWhiteTurn;
        }

        //remove highlighting
        selected = null;
    }
}
