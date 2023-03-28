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

    public bool isWhiteTurn;
    public ChessFigure selected;
    public bool[,] legalMoves;

    public int selectionR;
    public int selectionC;

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
            if (selectionR >= 0 && selectionC >= 0)
            {
                if (selected == null) SelectChessFigure(7 - selectionR, selectionC);
                else MoveChessFigure(7 - selectionR, selectionC);
            }
        }
    }

    private void UpdateSelection()
    {
        RaycastHit hit;
        float raycastDistance = 100.0f;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, raycastDistance))
        {
            selectionR = (int) hit.point.z;
            selectionC = (int) hit.point.x;
        }
        else
        {
            selectionR = -1;
            selectionC = -1;
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

        if (selectionR >= 0 && selectionC >= 0)
        {
            Debug.DrawLine(Vector3.forward * selectionR + Vector3.right * selectionC,
                Vector3.forward * (selectionR + 1) + Vector3.right * (selectionC + 1));
            Debug.DrawLine(Vector3.forward * selectionR + Vector3.right * (selectionC + 1),
                Vector3.forward * (selectionR + 1) + Vector3.right * selectionC);
        }
    }

    private void SpawnAllChessFigures()
    {
        //white pieces
        SpawnChessFigure(0, 0, 3); //king
        SpawnChessFigure(1, 0, 4); //queen
        SpawnChessFigure(2, 0, 0); SpawnChessFigure(2, 0, 7); //rooks
        SpawnChessFigure(3, 0, 2); SpawnChessFigure(3, 0, 5); //bishops
        SpawnChessFigure(4, 0, 1); SpawnChessFigure(4, 0, 6); //knights
        for (int i = 0; i < 8; i++)
        {
            SpawnChessFigure(5, 1, i); //pawn
        }

        //black pieces
        SpawnChessFigure(6, 7, 3); //king
        SpawnChessFigure(7, 7, 4); //queen
        SpawnChessFigure(8, 7, 0); SpawnChessFigure(8, 7, 7); //rooks
        SpawnChessFigure(9, 7, 2); SpawnChessFigure(9, 7, 5); //bishops
        SpawnChessFigure(10, 7, 1); SpawnChessFigure(10, 7, 6); //knights
        for (int i = 0; i < 8; i++)
        {
            SpawnChessFigure(11, 6, i); //pawn
        }
    }

    private void SpawnChessFigure(int index, int r, int c)
    {
        GameObject piece = Instantiate(pieces[index], GetTileCenter(7 - r, c), pieces[index].transform.rotation) as GameObject;
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
                    ChessFigure movingPiece = figurePositions[r, c];
                    ChessFigure originalPiece = figurePositions[i, j];

                    //make move without actually moving
                    figurePositions[i, j] = movingPiece;
                    figurePositions[r, c] = null;

                    legalMoves[i, j] = LegalMoveManager.Instance.CheckLegal();

                    //reset move
                    figurePositions[i, j] = originalPiece;
                    figurePositions[r, c] = movingPiece;
                }
                if (legalMoves[i, j])
                {
                    hasMove = true;
                }
            }
        }
        if (!hasMove)
        {
            selected = null;
            BoardHighlighting.Instance.HideHighlights();
            return;
        }
        selected = figurePositions[r, c];
        BoardHighlighting.Instance.HighlightAllowedMoves(legalMoves);
    }

    private void MoveChessFigure(int r, int c)
    {
        if (legalMoves[r, c])
        {
            if (selected.pieceType == PieceType.King && 
                Mathf.Abs(c - selected.CurrentC) == 2)
            {
                Castle(r, c);
            }

            ChessFigure square = figurePositions[r, c];
            if (square != null && square.isWhite != isWhiteTurn) //capture piece
            {
                active.Remove(square.gameObject);
                Destroy(square.gameObject);
            }

            figurePositions[selected.CurrentR, selected.CurrentC] = null;
            selected.transform.position = GetTileCenter(7 - r, c);
            selected.SetPosition(r, c);
            figurePositions[r, c] = selected;
            figurePositions[r, c].hasMoved = true;
            isWhiteTurn = !isWhiteTurn;
        }

        BoardHighlighting.Instance.HideHighlights();
        selected = null;
    }

    private void Castle(int r, int c)
    {
        if (c == 1)
        {
            ChessFigure rook = figurePositions[r, 0];
            rook.hasMoved = true;
            figurePositions[rook.CurrentR, rook.CurrentC] = null;
            rook.transform.position = GetTileCenter(7 - r, c + 1);
            rook.SetPosition(r, c + 1);
            figurePositions[r, c + 1] = rook;
        }
        else
        {
            ChessFigure rook = figurePositions[r, 7];
            rook.hasMoved = true;
            figurePositions[rook.CurrentR, rook.CurrentC] = null;
            rook.transform.position = GetTileCenter(7 - r, c - 1);
            rook.SetPosition(r, c - 1);
            figurePositions[r, c - 1] = rook;
        }
    }
}
