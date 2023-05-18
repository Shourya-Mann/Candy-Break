using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState// it can have certain different states for this game we have wait and move but wecan create stuff like booster on etc.
{
    wait,// set up in candy class
    move// set up in this class
}
public enum TileKind
{// this is the kind of tiles we can have in our scene
    Breakable, // jellys
    Blank,// empty spaces
    Normal
}

//dataholder class
[System.Serializable]// wee need this so unity can actually serialise this class
public class TileType
{// will hold a bit of info on the kind of background tile we have
    public int x;
    public int y;
    public TileKind tileKind;
}

public class Board : MonoBehaviour
{
    // variables to monitor the game state
    public GameState currentState = GameState.move;// thsi makes it so that whenever the game boots up its in the move state

    // defining variables for the grid 
    public int width;
    public int height;
    public int offSet;// offset at which new candy is created
    public GameObject tilePrefab; // the tile i wanna create
    public GameObject breakableTilePrefab;
    public GameObject destroyEffect; // game object to assign the effects to 
    // we want an array of tile type
    public TileType[] boardLayout;
    // defining 2d array, as empty container
    private bool[,] blankSpaces;
    private BackgroundTile[,] breakableTiles;
    // create a game object array to fill up with candy prefabs
    public GameObject[] candies;
    public GameObject[,] allCandy;
    public Candy currentCandy; // reference to last dot that was controlled

    private FindMatches findMatches;


    // Start is called before the first frame update
    void Start()
    {
        breakableTiles = new BackgroundTile[width, height];
        findMatches = FindObjectOfType<FindMatches>();
        // defines how big the attributes of array are
        blankSpaces = new bool[width, height];
        allCandy = new GameObject[width, height]; // makes it easier to understand how the movement is gonna happen
        SetUp(); // call the method/function to generate the board grid

    }

    public void GenerateBlankSpace()
    {
        for(int i = 0; i < boardLayout.Length; i++)
        { // we have taken the board layout array and turned it into booleans whose value is true and false
            if(boardLayout[i].tileKind == TileKind.Blank)
            {
                blankSpaces[boardLayout[i].x, boardLayout[i].y] = true;
            }
        }
    }

    public void GenerateBreakableTiles()
    {// look at all tiles in the layout,
        for (int i=0; i < boardLayout.Length; i++)
        {// if a tile is a breakable tile
            if (boardLayout[i].tileKind == TileKind.Breakable)
            {// create a breakable tile at that position
                Vector2 tempPostion = new Vector2(boardLayout[i].x, boardLayout[i].y);
                GameObject tile = Instantiate(breakableTilePrefab, tempPostion, Quaternion.identity);
                breakableTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<BackgroundTile>();
            }
        }
    }

    private void SetUp()
    {
        GenerateBlankSpace();
        GenerateBreakableTiles();
        for (int i = 0; i < width; i++) // for loop that goes left to right
        {
            for (int j = 0; j < height; j++)
            {
                if (!blankSpaces[i, j])
                {
                    Vector2 tempPosition = new Vector2(i, j + offSet); // defines postion to create candy + offset
                    //Vector2 tempPositiontile = new Vector2(i, j+offSet); // generates background grid at the same position using temppostion without offset
                                                                  // instead of getting unity to deal with the grid, we are creating it as a game object so we can manipulate it using code
                    GameObject backgroundTile = Instantiate(tilePrefab, tempPosition, Quaternion.identity) as GameObject; // creates tile at defined postion with default rotation
                                                                                                                              // setting parent of above game object to board object
                    backgroundTile.transform.parent = this.transform;
                    backgroundTile.name = "(" + i + "," + j + ")"; // changes the name of background tile to grid values     

                    // instantiating the candies
                    int candyToUse = Random.Range(0, candies.Length); // if you use random.range and dont specify type it will choose int values
                                                                      // before instantiation check to see if a match is made and if it is spawn a different candy
                    int maxIterations = 0; // iteration means how many times it goes thru the while loop
                    while (MatchesAt(i, j, candies[candyToUse]) && maxIterations < 100)
                    {
                        candyToUse = Random.Range(0, candies.Length);
                        maxIterations++;
                        //Debug.Log(maxIterations);
                    }
                    maxIterations = 0;

                    GameObject candy = Instantiate(candies[candyToUse], tempPosition, Quaternion.identity); // generate/spawn the cnady chosen by the random selector
                                                                                                            // due to offset candy has corret column but row is higher than it needs to be so we set the row and column where it has to actually be
                    candy.GetComponent<Candy>().row = j;
                    candy.GetComponent<Candy>().column = i;
                    candy.transform.parent = this.transform; // makes the candy a child of the background tile it spawned on
                    candy.name = "(" + i + "," + j + ")";  // the candy will have the right position label

                    allCandy[i, j] = candy; // creates a 2d array containing all the cnady created on the board
                }
            }
        }
    }

    // checks if any matches are created when candies spawn
    private bool MatchesAt(int column, int row, GameObject piece)
    {

        if (column > 1 && row > 1) // we are checking if column is greater than 1 which means i wont check for the first row or the first column properly
        {
            if (allCandy[column - 1, row] != null && allCandy[column - 2, row] != null)
            {
                if (allCandy[column - 1, row].tag == piece.tag && allCandy[column - 2, row].tag == piece.tag)
                {
                    return true;
                }
            }

            if (allCandy[column, row-1] != null && allCandy[column, row-2] != null)
            {
                if (allCandy[column, row - 1].tag == piece.tag && allCandy[column, row - 2].tag == piece.tag)
                {
                    return true;
                }
            }
        }
        else if (column <= 1 || row <= 1)
        {
            if (row > 1) // we meet he first section of the logic meaning this is for the first and seconfd column
            {
                if (allCandy[column, row - 1] != null && allCandy[column, row - 2] != null)
                {
                    if (allCandy[column, row - 1].tag == piece.tag && allCandy[column, row - 2].tag == piece.tag)
                    {
                        return true;
                    }
                }
            }
            if (column > 1) // we meet he second section of the logic meaning this is for the first and seconfd row
            {
                if (allCandy[column - 1, row] != null && allCandy[column - 2, row] != null)
                {
                    if (allCandy[column - 1, row].tag == piece.tag && allCandy[column - 2, row].tag == piece.tag)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private bool ColumnOrRow()
    {
        int numberHorizontal = 0;
        int numberVertical = 0;
        // assign first piece as the firt piece in the match and we assign the candy component of it to the firspiece variable
        Candy firstPiece = findMatches.currentMatches[0].GetComponent<Candy>();
        // we wanna go thru everything that is in the currentMatches and if it has the same row as the first piece we add a number to the horizontal and vice versa for the column
        // we also check to see if either of the numbers are = 5 then we make a color bomb otherwise we make a wrap bomb
        if (firstPiece != null) {
            foreach (GameObject currentPiece in findMatches.currentMatches)
            {
                Candy candy = currentPiece.GetComponent<Candy>();
                if (candy.row == firstPiece.row)
                {
                    numberHorizontal++;
                }
                if (candy.column == firstPiece.column)
                {
                    numberVertical++;
                }
            }
        }
        return (numberVertical == 5 || numberHorizontal == 5);
        // this means we will get a true if there is a five match for colorbomb
    }

    private void CheckToMakeBombs()
    {
        if (findMatches.currentMatches.Count == 4 || findMatches.currentMatches.Count == 7)
        {// makes column or row bomb
            findMatches.Checkbombs();
        }
        if (findMatches.currentMatches.Count == 5|| findMatches.currentMatches.Count == 8)
        {// makes color or wrap bomb, but to figure out which one to make we have to make a helper method
            if (ColumnOrRow())
            {
                //make a color bomb
                // is the current dot matched?
                if (currentCandy != null)
                {
                    if (currentCandy.isMatched)
                    {// check to see if its not already a color bomb
                        if (!currentCandy.isColorBomb)
                        {
                            currentCandy.isMatched = false;
                            currentCandy.MakeColorBomb();
                        }
                        else
                        {
                            if (currentCandy.othercandy != null)
                            {
                                Candy othercandy = currentCandy.othercandy.GetComponent<Candy>();
                                if (othercandy.isMatched)
                                {
                                    if (!othercandy.isColorBomb)
                                    {
                                        othercandy.isMatched = false;
                                        othercandy.MakeColorBomb();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                //make a wrap bomb
                // is the current dot matched?
                if (currentCandy != null)
                {
                    if (currentCandy.isMatched)
                    {// check to see if its not already a color bomb
                        if (!currentCandy.isWrapBomb)
                        {
                            currentCandy.isMatched = false;
                            currentCandy.MakeWrapBomb();
                        }
                        else
                        {
                            if (currentCandy.othercandy != null)
                            {
                                Candy othercandy = currentCandy.othercandy.GetComponent<Candy>();
                                if (othercandy.isMatched)
                                {
                                    if (!othercandy.isWrapBomb)
                                    {
                                        othercandy.isMatched = false;
                                        othercandy.MakeWrapBomb();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    private void destroyMatchesAt(int column, int row)
    {
        // helper method to create empty space for new candy to replace and calculate score
        if (allCandy[column, row].GetComponent<Candy>().isMatched)// checks to see if true
        {
            // How many elements are in the matched pieces list from findmatch
            if (findMatches.currentMatches.Count >= 4)
            {// checks to see if there is a match of 4
                CheckToMakeBombs();
            }
            // does a tile need to break?
            if (breakableTiles[column,row]!= null)
            {//if it does, give one damage
                breakableTiles[column, row].TakeDamage(1);
                if(breakableTiles[column,row].hitPoints <= 0)
                {
                    breakableTiles[column, row] = null;
                }
            }
            GameObject particle = Instantiate(destroyEffect,allCandy[column,row].transform.position, Quaternion.identity);
            Destroy(particle, 0.5f); // destroy the instantiated particle effect after 0.5 seconds
            Destroy(allCandy[column, row]);
            allCandy[column, row] = null;
        }
    }

    // method to derstroy all matches on the board
    public void DestroyMatches()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allCandy[i, j] != null)//this means if there is an object in the arry at that point we will do somthing else
                {
                    // check to see if piece exists at the columna nd row and check to see if can destroy matcfhes there
                    destroyMatchesAt(i, j);

                }
            }
        }
        findMatches.currentMatches.Clear();
        StartCoroutine(DecreaseRowCo2());//when matches are destoyed auotomatically tries to find if anything should be decreased and decrease it
    }

    private IEnumerator DecreaseRowCo2()
    {
        yield return new WaitForSeconds(0.3f);// wait a bit before the candy above drops down

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                // if the current spot isnt blank and is empty
                if (!blankSpaces[i, j] && allCandy[i,j] == null)
                {
                    //loop from the space above to the top of the column
                    for(int k = j+1; k<height; k++)
                    {

                        // if that dot is found 
                        if (allCandy[i, k] != null)
                        {
                            // move that candy to this empty space
                            allCandy[i, k].GetComponent<Candy>().row = j;
                            //set thats spot to be null
                            allCandy[i, k] = null;
                            //break out of the loop
                            break;
                        }
                    }
                }
            }
        }
        yield return new WaitForSeconds(0.3f);
        StartCoroutine(FillBoardCo());
    }
  /*  private IEnumerator DecreaseRowCo()// to calculate how many emty spaces are in each column so we cal use the method to get the column to collapse
    {
        int nullCount = 0;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allCandy[i, j] == null)
                {
                    nullCount++;
                }
                else if (nullCount > 0)
                {
                    allCandy[i, j].GetComponent<Candy>().row -= nullCount;
                    allCandy[i, j] = null;// so when next dot fills in it keeps correct postion in the array
                }
            }
            nullCount = 0; //reseting the null count to zero after checking for the whole column
        }
        yield return new WaitForSeconds(0.4f);
        StartCoroutine(FillBoardCo());
    }*/

    // new method to refill the board with two helper methods
    //method to spawn in new pieces
    private void refillBoard()
    {
        // has to run after we decrease the row
        // checks thru all pieces, if any are null instantiates a new candy there
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                if (allCandy[i, j] == null && !blankSpaces[i,j])
                {
                    Vector2 tempPostion = new Vector2(i, j+offSet);
                    int candyToUse = Random.Range(0, candies.Length);
                    GameObject piece = Instantiate(candies[candyToUse], tempPostion, Quaternion.identity);
                    allCandy[i, j] = piece;
                    // to get the new candy slide in from an offset check the candy script for the part hat kinked to this in movepieces function
                    piece.GetComponent<Candy>().row = j;
                    piece.GetComponent<Candy>().column = i;
                }
            }
        }
    }

    // method to check for matches 
    private bool MatchesOnBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allCandy[i, j] != null)
                {
                    if (allCandy[i, j].GetComponent<Candy>().isMatched)
                    { 
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private IEnumerator FillBoardCo()
    {
        // checks to see if there are matches on the board does the process again till there are none then player is allowed to move

        refillBoard();
        yield return new WaitForSeconds(0.4f);
        
        while (MatchesOnBoard())
        {
            DestroyMatches();
           //yield return new WaitForSeconds(0.5f);
            //DestroyMatches();
        }
        findMatches.currentMatches.Clear();
        currentCandy = null;
        yield return new WaitForSeconds(0.4f);// to add a half second pause
        currentState = GameState.move;
    }

    // helper routines for detecting deadlock

    // helper routine to switch pieces
    private void SwitchPieces(int column, int row, Vector2 direction) 
        // arguments neede for column, row and a direction to switch pieces in
    {
        // take the second piece and save it in a holder
        GameObject holder = allCandy[column + (int)direction.x, row + (int)direction.y] as GameObject; // we save as game object cause unity vcan act wierd if you save someting from a 2d array as a single object 
        // direction is a float so we have to change it to an integer and add it to the column and row values
        // switching the first dot to be the second postion
        allCandy[column + (int)direction.x, row + (int)direction.y] = allCandy[column, row];
        // set the second dot to be the first dot
        allCandy[column, row] = holder;
    }

    // helper method to check matches on board and return a boolean value.
    private bool  CheckForMatches()// takes no argument cause it checks the whole board
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allCandy[i, j] != null)
                {
                    if (allCandy[i+1,j]!= null && allCandy[i+2, j]!= null)
                    {

                    }
                }
            }
        }
        return false;

    }
}
  