using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    [SerializeField] public int isWhite;
    [SerializeField] Vector2 position;
    [SerializeField] public PieceData.Type type;
    [SerializeField] public bool hasMoved = false;
    [SerializeField] public bool hasCastled = false;
    
    public PieceData pieceData;

    private void Awake() => pieceData = new PieceData(gameObject, isWhite, position, type);

    private void Start()
    {
        position = pieceData.position;
    }
}

