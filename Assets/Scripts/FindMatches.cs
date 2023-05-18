using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq; // allows you to join to lists together
// similar logic to the candy class, we need access to board class 
public class FindMatches : MonoBehaviour
{
    private Board board;
    // list of game objects to detect if its a match 3 match 4 or match five
    public List<GameObject> currentMatches = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        
    }

    // calling the coroutine, you cant call a couroutine from another script, but you can call a method that calls a coroutine from another script.
    public void FindAllMatches()
    {
        StartCoroutine(FindAllMatchesCo());
    }

    private List<GameObject> isWrapBomb(Candy candy1, Candy candy2, Candy candy3)
    {
        List<GameObject> currentcandies = new List<GameObject>();

        if (candy1.isWrapBomb)
        {// works for pieces in the middle
            currentMatches.Union(getWrapPieces(candy1.column, candy1.row));
        }
        if (candy2.isWrapBomb)
        { //checks for if there is a column bomb inside match
            currentMatches.Union(getWrapPieces(candy2.column, candy2.row));
        }
        if (candy3.isWrapBomb)
        { //checks for if there is a column bomb inside match
            currentMatches.Union(getWrapPieces(candy3.column, candy3.row));
        }

        return currentcandies;
    }


    private List<GameObject> isRowBomb(Candy candy1, Candy candy2, Candy candy3)// creates a list of current candy and returns those current candy
    {// the 3 candies can represent the left, current, and right and the up, current, and down.
        List<GameObject> currentcandies = new List<GameObject>();

        if (candy1.isRowBomb)
        {// works for pieces in the middle
            currentMatches.Union(GetRowPieces(candy1.row));
        }
        if (candy2.isRowBomb)
        { //checks for if there is a column bomb inside match
            currentMatches.Union(GetRowPieces(candy2.row));
        }
        if (candy3.isRowBomb)
        { //checks for if there is a column bomb inside match
            currentMatches.Union(GetRowPieces(candy3.row));
        }
        
        return currentcandies;
     }

    private List<GameObject> isColumnBomb(Candy candy1, Candy candy2, Candy candy3)// creates a list of current candy and returns those current candy
    {// the 3 candies can represent the left, current, and right and the up, current, and down.
        List<GameObject> currentcandies = new List<GameObject>();

        if (candy1.isColumnBomb)
        {// works for pieces in the middle
            currentMatches.Union(GetColumnPieces(candy1.column));
        }
        if (candy2.isColumnBomb)
        { //checks for if there is a column bomb inside match
            currentMatches.Union(GetColumnPieces(candy2.column));
        }
        if (candy3.isColumnBomb)
        { //checks for if there is a column bomb inside match
            currentMatches.Union(GetColumnPieces(candy3.column));
        }

        return currentcandies;
    }

    private void AddToListAndMatch(GameObject candy)
    {
        if (!currentMatches.Contains(candy))
        {
            currentMatches.Add(candy);
        }
        candy.GetComponent<Candy>().isMatched = true;
    }
    private void getNearbyPieces(GameObject candy1, GameObject candy2, GameObject candy3)
    {
        AddToListAndMatch(candy1);
        AddToListAndMatch(candy2);
        AddToListAndMatch(candy3);
    }
    private IEnumerator FindAllMatchesCo()
    {
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < board.width; i++) // for loop that goes left to right
        {
            for (int j = 0; j < board.height; j++)
            {
                GameObject currentCandy = board.allCandy[i, j];

                if (currentCandy != null)// makes sure that the dot we are looking at exists 
                {
                    Candy currentDotCandy = currentCandy.GetComponent<Candy>(); // to limit how many times we have to call the candies using the getcomponent function

                    // for horizontal matches
                    if (i > 0 &&i< board.width - 1) //meaning i is between the second column to the second last column
                    {
                        //create left and right game object
                        GameObject leftcandy = board.allCandy[i - 1, j];
                        GameObject rightcandy = board.allCandy[i + 1, j];
                      


                        if (leftcandy != null && rightcandy != null)// checks to see if the left and right candy are not destroyed
                        {
                            Candy rightDotCandy = rightcandy.GetComponent<Candy>();
                            Candy leftDotCandy = leftcandy.GetComponent<Candy>();
                            if (leftcandy.tag == currentCandy.tag && rightcandy.tag == currentCandy.tag) //  checks to see if adjacent candy has same tag
                            {// checking for row bombs and column bombs and adding the pieces
                                currentMatches.Union(isRowBomb(leftDotCandy, currentDotCandy, rightDotCandy));
                                currentMatches.Union(isColumnBomb(leftDotCandy, currentDotCandy, rightDotCandy));
                                currentMatches.Union(isWrapBomb(leftDotCandy, currentDotCandy, rightDotCandy));


                                getNearbyPieces(leftcandy, currentCandy, rightcandy);
                                
                            }
                        }
                    }

                    // for vertical matches
                    if (j > 0 && j < board.height - 1) //meaning i is between the second column to the second last column
                    {
                        //create left and right game object
                        GameObject upcandy = board.allCandy[i, j+1];
                        GameObject downcandy = board.allCandy[i, j-1];
                     
                        if (upcandy != null && downcandy != null)// checks to see if the left and right candy are not destroyed
                        {
                            Candy downDotCandy = downcandy.GetComponent<Candy>();
                            Candy upDotCandy = upcandy.GetComponent<Candy>(); // to limit how many times we have to call the candies using the getcomponent function
                            if (upcandy.tag == currentCandy.tag && downcandy.tag == currentCandy.tag)
                            {
                                currentMatches.Union(isColumnBomb(upDotCandy, currentDotCandy, downDotCandy));
                                currentMatches.Union(isRowBomb(upDotCandy, currentDotCandy, downDotCandy));
                                currentMatches.Union(isWrapBomb(upDotCandy, currentDotCandy, downDotCandy));


                                getNearbyPieces(upcandy, currentCandy, downcandy);
                            }
                        }
                    }
                }
            }
        }
    }

    public void MatchPiecesOfColor(string color)// the tags we have assigned to each candy is saved as a string so we are passing it in the method
    {
        for(int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                // check if that piece exists
                if(board.allCandy[i,j]!= null)
                {
                    // check the tag on that candy
                    if(board.allCandy[i,j].tag== color)
                    {
                        // set that candy to be matched
                        board.allCandy[i, j].GetComponent<Candy>().isMatched = true;
                    }
                }
            }
        }
    }

    List<GameObject> getWrapPieces(int column, int row)
    {
        List<GameObject> candies = new List<GameObject>();
        for (int i = column - 1; i <= column + 1; i++) // for loop that goes left to right
        {
            for (int j = row - 1; j <= row + 1; j++)
            {// check if the piece is inside the board, so it doesnt check for pieces that arebeyod the edge
                if (i >= 0 && i < board.width && j >= 0 && j < board.height)
                {
                    candies.Add(board.allCandy[i, j]);
                    board.allCandy[i, j].GetComponent<Candy>().isMatched = true;
                }

            }
        }
        return candies;
    }

    // method to return a list of matched candy
    List<GameObject> GetColumnPieces(int column)
    {
        List<GameObject> candies = new List<GameObject>();
        // we wanna be able to cycle thru all our pieces that in the allCandy array and check to see if they are in the colmn tht we are in and we count them to be matched and add to our list
        for (int i = 0; i< board.height; i++)
        {
            if(board.allCandy[column,i]!= null)
            {
                candies.Add(board.allCandy[column, i]); // adds to the list
                board.allCandy[column, i].GetComponent<Candy>().isMatched = true;
            }
        }
        return candies;
    }

    List<GameObject> GetRowPieces(int row)
    {
        List<GameObject> candies = new List<GameObject>();
        // we wanna be able to cycle thru all our pieces that in the allCandy array and check to see if they are in the row tht we are in and we count them to be matched and add to our list
        for (int i = 0; i < board.width; i++)
        {
            if (board.allCandy[i, row] != null)
            {
                candies.Add(board.allCandy[i, row]); // adds to the list
                board.allCandy[i, row].GetComponent<Candy>().isMatched = true;
            }
        }
        return candies;
    }

    public void Checkbombs()
    {// did the player move something
        if(board.currentCandy != null)
        { // is thepiece they moved currently matched 
            if (board.currentCandy.isMatched)
            {
                // make it unmatched
                board.currentCandy.isMatched = false;
               
                if ((board.currentCandy.swipeAngle >-45 && board.currentCandy.swipeAngle <= 45)
                    || (board.currentCandy.swipeAngle <-135 || board.currentCandy.swipeAngle >=135))
                {// condition for horizontal swipe met
                    board.currentCandy.MakeRowBomb();
                }
                else
                {
                    board.currentCandy.MakeColumnBomb();
                }

                // Is the other piece matched
            } else if (board.currentCandy.othercandy != null)
            {
                // othercandy is gameobject of type Candy, but othercandy is just a gameobject, which means we need to get the component of the candyscript from it
                Candy otherCandy = board.currentCandy.othercandy.GetComponent<Candy>(); 
                // is the other candy matched
                if (otherCandy.isMatched)
                {
                    // make it unmatched
                    otherCandy.isMatched = false;

                    // we still check current candy angle cause we set othercandy = to to the other piece that was making the match
                    if ((board.currentCandy.swipeAngle > -45 && board.currentCandy.swipeAngle <= 45)
                    || (board.currentCandy.swipeAngle < -135 || board.currentCandy.swipeAngle >= 135))
                    {// condition for horizontal swipe met
                        otherCandy.MakeRowBomb();
                    }
                    else
                    {
                        otherCandy.MakeColumnBomb();
                    }
                }
            }
        }

    }
}
