using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Candy : MonoBehaviour
{
    [Header("Board Variables")]
    public int column; // reference to column and row
    public int row;
    public int previousColumn;
    public int previousRow;
    public int targetX;
    public int targetY;
    public bool isMatched = false; // bool to check if same sprites are next to each other

    private FindMatches findMatches; // reference to the find matches scripts
    private Board board; // reference to the Board script
    public GameObject othercandy; // reference to other candy
    private Vector2 firstTouchPostion;
    private Vector2 finalTouchPostion;
    private Vector2 tempPosition;

    [Header("Swipe Stuff")]
    public float swipeAngle = 0;
    public float swipeResist = 0.5f;

    [Header("Powerup stuff")]
    public bool isColorBomb;
    public bool isColumnBomb;
    public bool isRowBomb;
    public bool isWrapBomb;
    public GameObject rowArrow;
    public GameObject columnArrow;
    public GameObject colorBomb;
    public GameObject wrapBomb;


    // Start is called before the first frame update
    void Start()
    {
        isColumnBomb = false;
        isRowBomb = false;
        isColorBomb = false;
        isWrapBomb = false;
        // capital F Findmatches is the method, lower case the the referce variable
        board = FindObjectOfType<Board>(); //we do this cause there is one baord, if there were more then it would be slightly comlicated
        findMatches = FindObjectOfType<FindMatches>();
       // targetX = (int)transform.position.x; // cast the target x to an integer
        //targetY = (int)transform.position.y;
        //row = targetY;
        //column = targetX;
        //previosRow = row;
       // previousColumn = column;
    }

    // this if for testing and debug only
    private void OnMouseOver()// we want to turn a single piece into a row bomb or a column bomb
    {
        if (Input.GetMouseButtonDown(1))
        {
            isWrapBomb = true;
            GameObject wrap = Instantiate(wrapBomb, transform.position, Quaternion.identity);
            // because the pieces slide if they move, boost arrow will remain at same spot so we make the arrow a child of the piece we spawned it on
            wrap.transform.parent = this.transform;
        }
    }
    // Update is called once per frame
    void Update()
    {
        

        targetX = column; 
        targetY = row; // now wehn we change the column of the piece we change the target x and the same applies to rows

        // to get the peices to switch in the X dirertion
        if (Mathf.Abs(targetX - transform.position.x) > 0.1)
        {
            //move towards target
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPosition, 1f); // lerp is a movement method that we use here
            if (board.allCandy[column, row] != this.gameObject)// to avoid any wierd overlap of game objects
            {
                board.allCandy[column, row] = this.gameObject;
            }
            findMatches.FindAllMatches(); //checks for match when moving towards the target 
        }
        else
        {
            // directly set the position
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = tempPosition;
            board.allCandy[column, row] = this.gameObject;

        }

        // to get the peices to switch in the Y dirertion
        if (Mathf.Abs(targetY - transform.position.y) > 0.1)
        {
            //move towards target
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPosition, 1f); // lerp is a movement method that we use here
            if (board.allCandy[column, row] != this.gameObject)// to avoid any wierd overlap of game objects
            {
                board.allCandy[column, row] = this.gameObject;
            }
            findMatches.FindAllMatches();

        }
        else
        {
            // directly set the position
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = tempPosition;
        }
    }

    public IEnumerator CheckMoveCo()
    {
        if (isColorBomb)
        {
            //this piece is a colorbomb and the other piece is the color to destroy
            findMatches.MatchPiecesOfColor(othercandy.tag);
            isMatched = true;
        } else if (othercandy.GetComponent<Candy>().isColorBomb)
        {
            //the other piece is a colorbomb and this piece is the color to destroy
            findMatches.MatchPiecesOfColor(this.gameObject.tag);
            othercandy.GetComponent<Candy>().isMatched = true;
        }
        yield return new WaitForSeconds(0.6f);
        if(othercandy != null)
        {
            if (!isMatched && !othercandy.GetComponent<Candy>().isMatched)
            {
                othercandy.GetComponent<Candy>().row = row;
                othercandy.GetComponent<Candy>().column = column;
                row = previousRow;
                column = previousColumn;
                yield return new WaitForSeconds(0.5f);
                board.currentCandy = null;
                board.currentState = GameState.move; // changes state to move after pieces switch back
            }
            else // need to check it when othercandy is not null
            {
                board.DestroyMatches();
            }
           //othercandy = null;
        }
        
    }

    private void OnMouseDown()
    {
        if (board.currentState == GameState.move) // this way the mouse input is only taken when the game is in the move state
        { 
             firstTouchPostion = Camera.main.ScreenToWorldPoint(Input.mousePosition);// need camera to main statemtn to change the input to the game world coordinates intead of the pixel coordinate
             //Debug.Log(firstTouchPostion);
      
        }
    }

    private void OnMouseUp()
    {
        if (board.currentState == GameState.move) 
        { 
            finalTouchPostion = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Calculateangle();
        }
    }

    void Calculateangle()
    {
        // if statment ensures that some distance has to be moved in order for a swipe to be considered
        if (Mathf.Abs(finalTouchPostion.y - firstTouchPostion.y) > swipeResist || Mathf.Abs(finalTouchPostion.x - firstTouchPostion.x) > swipeResist)
        {
            board.currentState = GameState.wait;// changes our gamestate to wait after we calculate our angle

            float dy = finalTouchPostion.y - firstTouchPostion.y;
            float dx = finalTouchPostion.x - firstTouchPostion.x;
            swipeAngle = Mathf.Atan2(dy, dx) * 180 / Mathf.PI;// calculates angle between the drag and release of mouse
            //Debug.Log(swipeAngle);
            board.currentCandy = this; // when we select a candy it makes the current candy the one that was selected
            MovePieces();

        }
        else
        {
            board.currentState = GameState.move;
        }
    }
    void MovePiecesActual(Vector2 direction)
    {
        othercandy = board.allCandy[column + (int)direction.x, row + (int)direction.y]; // the dot we wanna grab
        previousRow = row;
        previousColumn = column;
        if (othercandy != null)
        {
            othercandy.GetComponent<Candy>().column += -1 * (int)direction.x;
            othercandy.GetComponent<Candy>().row += -1 * (int)direction.y;
            column += (int)direction.x;
            row += (int)direction.y;
            StartCoroutine(CheckMoveCo());
        }
        else
        {
            board.currentState = GameState.move;
        }
    }

    void MovePieces()
    {
        if(swipeAngle> -45 && swipeAngle <= 45 && column <board.width-1) // right swipe
        {
            MovePiecesActual(Vector2.right);
        }
        else if (swipeAngle > 45 && swipeAngle <= 135 && row< board.height-1) // up swipe
        {
            MovePiecesActual(Vector2.up);
        }
        else if ((swipeAngle > 135 || swipeAngle <-135) && column>0) // left swipe ... we do or because there is no number that will be greater than 135 and less than -135
        {
            MovePiecesActual(Vector2.left);
        }
        else if (swipeAngle < -45 && swipeAngle >= -135 && row>0 ) //downswipe
        {
            MovePiecesActual(Vector2.down);
        }
        board.currentState = GameState.move;
    }

    void FindMatches()
    {
        // for horizontal matches
        if (column > 0 && column < board.width - 1)
        {
            GameObject leftcandy1 = board.allCandy[column - 1, row];
            GameObject rightcandy1 = board.allCandy[column + 1, row];
            if (leftcandy1 != null && rightcandy1 != null) 
            {
                // makes it so that dots dont have to unnecesarily check for candies that dont exist dois this is defensive programing cause it checks for every situation 
                 if (leftcandy1.tag == this.gameObject.tag && rightcandy1.tag == this.gameObject.tag) // checks to see if the candies on the right and left match
                 {
                     leftcandy1.GetComponent<Candy>().isMatched = true;
                     rightcandy1.GetComponent<Candy>().isMatched = true;
                     isMatched = true;
                 }
        }
        }

        // for vertical matches
        if (row > 0 && row < board.height - 1)
        {
            GameObject upcandy1 = board.allCandy[column, row+1];
            GameObject downcandy1 = board.allCandy[column, row-1];
            if (upcandy1 != null && downcandy1 != null)
            {
                if (upcandy1.tag == this.gameObject.tag && downcandy1.tag == this.gameObject.tag) // checks to see if the candies above and below match
                {
                    upcandy1.GetComponent<Candy>().isMatched = true;
                    downcandy1.GetComponent<Candy>().isMatched = true;
                    isMatched = true;
                }
            }
        }
    }

    public void MakeRowBomb()
    {
        isRowBomb = true;
        GameObject arrow = Instantiate(rowArrow, transform.position, Quaternion.identity);
        // because the pieces slide if they move, boost arrow will remain at same spot so we make the arrow a child of the piece we spawned it on
        arrow.transform.parent = this.transform;
    }

    public void MakeColumnBomb()
    {
        isColumnBomb = true;
        GameObject arrow = Instantiate(columnArrow, transform.position, Quaternion.identity);
        // because the pieces slide if they move, boost arrow will remain at same spot so we make the arrow a child of the piece we spawned it on
        arrow.transform.parent = this.transform;
    }

    public void MakeColorBomb()
    {
        isColorBomb = true;
        GameObject color = Instantiate(colorBomb, transform.position, Quaternion.identity);
        // because the pieces slide if they move, boost arrow will remain at same spot so we make the arrow a child of the piece we spawned it on
        color.transform.parent = this.transform;
    }

    public void MakeWrapBomb()
    {
        isWrapBomb = true;
        GameObject wrap = Instantiate(wrapBomb, transform.position, Quaternion.identity);
        // because the pieces slide if they move, boost arrow will remain at same spot so we make the arrow a child of the piece we spawned it on
        wrap.transform.parent = this.transform;
    }
}
