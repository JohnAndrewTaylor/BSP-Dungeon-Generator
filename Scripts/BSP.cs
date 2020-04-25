// Copyright (c) 2020 John Andrew Taylor
// Version: 1.0

// This code randomly generates a set of dungeon rooms
// using Binary Space Partitioning. The rooms are drawn in
// Unity with the use of tilemaps.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BSP : MonoBehaviour
{
    // Tilemaps
    public Tilemap tilemap_floor;
    public Tilemap tilemap_walls;

    // Tiles
    public TileBase floorTile;
    public TileBase backgroundTile;

    public TileBase upperBorderCornerL;
    public TileBase upperBorder;
    public TileBase upperBorderCornerR;
    public TileBase upperWallCornerL;
    public TileBase upperWall0;
    public TileBase upperWall1;
    public TileBase upperWall2;
    public TileBase upperWall3;
    public TileBase upperWall4;
    public TileBase upperWallGate;
    public TileBase upperWallCornerR;

    public TileBase leftBorder;

    public TileBase rightBorder;

    public TileBase lowerBorderCornerL;
    public TileBase lowerBorder;
    public TileBase lowerBorderCornerR;

    public TileBase crate;
    public TileBase chest;
    public TileBase knightRed;
    public TileBase knightBlue;
    public TileBase knightGreen;
    public TileBase knightPurple;
    public TileBase rocks;
    //

    public int xDim = 64;
    public int yDim = 64;

    public const int MIN_SIZE = 10;
    public const int MAX_SIZE = 20;

    public List<Rect> drawnRooms = new List<Rect>();
    public List<Rect> halls = new List<Rect>();

    public class Room
    {
        // Rectangular representation of room
        public Rect roomRect;

        // Children of the room generated in split()
        public Room leftChild;
        public Room rightChild;

        public Room(Rect newRect)
        {
            roomRect = newRect;
        }

        public bool split()
        {
            // Should not split rooms that have already been split
            if (leftChild != null || rightChild != null)
            {
                return false;
            }

            // Randomly pick if the split will be horizontal or vertical
            bool horSplit = (Random.value < 0.5f);

            // To prevent splitting a room in one way too much
            if (roomRect.width / roomRect.height >= 1.25)
            {
                horSplit = false;
            }
            else if (roomRect.height / roomRect.width >= 1.25)
            {
                horSplit = true;
            }

            // Determine if an appropriate size is impossible
            int maxDimension = (horSplit ? (int)roomRect.height : (int)roomRect.width) - MIN_SIZE;
            if (maxDimension < MIN_SIZE)
            {
                return false;
            }

            // Calculate where the split will occur
            int split = Random.Range(MIN_SIZE, maxDimension);

            // Split the room into two children
            if (horSplit)
            {
                leftChild = new Room(new Rect(roomRect.x, roomRect.y, roomRect.width, split));
                rightChild = new Room(new Rect(roomRect.x, roomRect.y + split, roomRect.width, roomRect.height - split));
            }
            else
            {
                leftChild = new Room(new Rect(roomRect.x, roomRect.y, split, roomRect.height));
                rightChild = new Room(new Rect(roomRect.x + split, roomRect.y, roomRect.width - split, roomRect.height));
            }

            // Split was successful
            return true;
        }

        // Creates all room by recursing through children
        public void createRooms()
        {
            // Current room is not a leaf
            if (leftChild != null || rightChild != null)
            {
                // Recurse through valid children
                if (leftChild != null)
                {
                    leftChild.createRooms();
                }
                if (rightChild != null)
                {
                    rightChild.createRooms();
                }
            }
            else
            {
                int roomWidth = (int)Random.Range(roomRect.width / 2, roomRect.width - 2);
                int roomHeight = (int)Random.Range(roomRect.height / 2, roomRect.height - 2);
                int roomX = (int)Random.Range(1, roomRect.width - roomWidth - 1);
                int roomY = (int)Random.Range(1, roomRect.height - roomHeight - 1);

                roomRect = new Rect(roomRect.x + roomX, roomRect.y + roomY, roomWidth, roomHeight);
            }
        }
    }

    public void recurseBSP(Room parent)
    {
        // Fail if the input was null
        if (parent == null)
        {
            return;
        }

        // Should not recurse on already split rooms
        if(parent.leftChild != null || parent.rightChild != null)
        {
            return;
        }

        // Only attempt a split if the room is large enough
        if (parent.roomRect.width > MAX_SIZE || parent.roomRect.height > MAX_SIZE)
        {
            // Only recurse if the split was successful
            if (parent.split())
            {
                recurseBSP(parent.leftChild);
                recurseBSP(parent.rightChild);
            }
        }
    }

    public void drawLeafs(Room parent)
    {
        Debug.Log("Call of draw");
        
        // Fail if the input was null
        if (parent == null)
        {
            return;
        }

        // Draw the room if it is a leaf, otherwise recurse
        if(parent.leftChild == null && parent.rightChild == null)
        {

            Debug.Log("Drew room " + parent + " in rectangle " + parent.roomRect);
            drawnRooms.Add(parent.roomRect);

            // Draw the floor of the room
            for (int i = (int)parent.roomRect.x - 1; i < (int)parent.roomRect.x + (int)parent.roomRect.width + 1; i++)
            {
                for (int j = (int)parent.roomRect.y; j < (int)parent.roomRect.y + (int)parent.roomRect.height; j++)
                {
                    tilemap_floor.SetTile(new Vector3Int(i,j,0), floorTile);
                }
            }

            // Set x and y for convenience
            int x = (int)parent.roomRect.x;
            int y = (int)parent.roomRect.y;

            // Draw the upper borders
            tilemap_walls.SetTile(new Vector3Int(x - 1, y + (int)parent.roomRect.height, 0), upperBorderCornerL);
            tilemap_walls.SetTile(new Vector3Int(x + (int)parent.roomRect.width, y + (int)parent.roomRect.height, 0), upperBorderCornerR);

            for (int i = x; i < x + (int)parent.roomRect.width; i++)
            {
                tilemap_walls.SetTile(new Vector3Int(i, y + (int)parent.roomRect.height, 0), upperBorder);
            }

            // Draw the upper walls
            TileBase[] upperWalls = { upperWall0, upperWall1, upperWall2, upperWall3, upperWall4, upperWallGate };
            tilemap_walls.SetTile(new Vector3Int(x - 1, y + (int)parent.roomRect.height - 1, 0), upperWallCornerL);
            tilemap_walls.SetTile(new Vector3Int(x + (int)parent.roomRect.width, y + (int)parent.roomRect.height - 1, 0), upperWallCornerR);

            for (int i = x; i < x + (int)parent.roomRect.width; i++)
            {
                // Selects a random wall tile, with a preference for plain
                TileBase selWall = upperWalls[Random.Range(0, upperWalls.Length)];
                if (Random.value < 0.7f)
                {
                    selWall = upperWall0;
                }
                tilemap_walls.SetTile(new Vector3Int(i, y + (int)parent.roomRect.height - 1, 0), selWall);
            }

            // Draw the lower borders
            tilemap_walls.SetTile(new Vector3Int(x - 1, y, 0), lowerBorderCornerL);
            tilemap_walls.SetTile(new Vector3Int(x + (int)parent.roomRect.width, y, 0), lowerBorderCornerR);

            for (int i = x; i < x + (int)parent.roomRect.width; i++)
            {
                tilemap_walls.SetTile(new Vector3Int(i, y, 0), lowerBorder);
            }

            // Draw the left borders
            for(int i = y+1; i < y + (int)parent.roomRect.height-1; i++)
            {
                tilemap_walls.SetTile(new Vector3Int(x-1, i, 0), leftBorder);
            }

            // Draw the right borders
            for (int i = y + 1; i < y + (int)parent.roomRect.height - 1; i++)
            {
                tilemap_walls.SetTile(new Vector3Int(x + (int)parent.roomRect.width, i, 0), rightBorder);
            }

            // Add decorations
            TileBase[] decorations = {crate, chest, knightRed, knightBlue, knightGreen, knightPurple, rocks};
            for (int i = (int)parent.roomRect.x; i < (int)parent.roomRect.x + (int)parent.roomRect.width; i++)
            {
                for (int j = (int)parent.roomRect.y + 1; j < (int)parent.roomRect.y + (int)parent.roomRect.height - 1; j++)
                {
                    if(Random.value < 0.05f)
                    {
                        TileBase selDec = decorations[Random.Range(0, decorations.Length)];
                        tilemap_walls.SetTile(new Vector3Int(i, j, 0), selDec);
                    }
                }
            }


        }
        else
        {
            drawLeafs(parent.leftChild);
            drawLeafs(parent.rightChild);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Set background color
        for(int i = 0; i < xDim; i++)
        {
            for(int j = 0; j < yDim; j++)
            {
                tilemap_floor.SetTile(new Vector3Int(i, j, 0), backgroundTile);
            }
        }

        // Represents the overal dimensions of the level
        Room rootLevel = new Room(new Rect(0, 0, xDim, yDim));

        // Starts recursion at root
        recurseBSP(rootLevel);
        Debug.Log("Done with BSP");

        // Creates rooms recursively from the root
        rootLevel.createRooms();
        Debug.Log("Done with room creation");

        // Go through created rooms and draw the leafs
        drawLeafs(rootLevel);
        Debug.Log("Done with drawing");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
