using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Tile_Information : MonoBehaviour
{
    [field: SerializeField] public TileState tileState { get; set; }
    [SerializeField] private SpriteRenderer mySpriteRenderer = null;
    [SerializeField] private Sprite holeSprite = null;

    private void Awake()
    {
        TryGetComponent<SpriteRenderer>(out mySpriteRenderer);
    }

    private void Start()
    {
        if (tileState == TileState.Hole) mySpriteRenderer.sprite = holeSprite;
    }
}
