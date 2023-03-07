using System;
using System.IO;

namespace DEU_CENG_CHESS
{
    class DEU_CENG_CHESS
    {
        //Global bools that holds the information of if the kings or rooks have moved before to control if castling is possible
        static bool firstMoveBlueK = false, firstMoveRedK = false, firstMoveRedLR = false, firstMoveRedRR = false,
                    firstMoveBlueLR = false, firstMoveBlueRR = false;

        //A virtual board that holds every piece's threat map
        static bool[,] blueThreat = new bool[8, 8];
        static bool[,] redThreat = new bool[8, 8];
        static bool checkOnRed = false, checkOnBlue = false;

        //Global bool for attaining every column en passantable situtation
        static bool[] enPassant = new bool[8];
        static bool enPassantNotation = false;

        static bool shortCastleBlue = false, shortCastleRed = false, longCastleBlue = false, longCastleRed = false;
        static bool capture = false;
        static bool promotion = false;

        //Writes the static screen and returns default values of the table array.
        static char[,,] Start()
        {
            //Static Board
            Console.Write(" +------------------------+\n" +
                          "8| .  .  .  .  .  .  .  . |\n" +
                          " |                        |\n" +
                          "7| .  .  .  .  .  .  .  . |\n" +
                          " |                        |\n" +
                          "6| .  .  .  .  .  .  .  . |\n" +
                          " |                        |\n" +
                          "5| .  .  .  .  .  .  .  . |\n" +
                          " |                        |\n" +
                          "4| .  .  .  .  .  .  .  . |\n" +
                          " |                        |\n" +
                          "3| .  .  .  .  .  .  .  . |\n" +
                          " |                        |\n" +
                          "2| .  .  .  .  .  .  .  . |\n" +
                          " |                        |\n" +
                          "1| .  .  .  .  .  .  .  . |\n" +
                          " +------------------------+\n" +
                          "   a  b  c  d  e  f  g  h    ");

            //3rd dimension is for differentiating between players' pieces.
            //'+' is blue, '-' is red, '*' is empty
            char[,,] table = new char[8, 8, 2];

            //Default values of the table array
            for (int i = 0; i < table.GetLength(0); i++)
            {
                for (int j = 0; j < table.GetLength(1); j++)
                {
                    table[i, j, 0] = '.';
                    table[i, j, 1] = '*';
                }
            }
            //Places pawns to 2nd and 7th lines.
            for (int i = 0; i < table.GetLength(1); i++)
            {
                table[1, i, 1] = '-';
                table[1, i, 0] = 'P';
            }
            for (int i = 0; i < table.GetLength(1); i++)
            {
                table[6, i, 1] = '+';
                table[6, i, 0] = 'P';
            }
            //Places rest of the pieces on 1st and 8th lines.
            for (int i = 0; i < 8; i += 7)
            {
                if (i == 0)
                    for (int j = 0; j < 8; j++)
                        table[0 + i, 0 + j, 1] = '-';
                else
                    for (int j = 0; j < 8; j++)
                        table[0 + i, 0 + j, 1] = '+';

                table[0 + i, 0, 0] = 'R'; table[0 + i, 1, 0] = 'N'; table[0 + i, 2, 0] = 'B'; table[0 + i, 3, 0] = 'Q'; table[0 + i, 4, 0] = 'K';
                table[0 + i, 5, 0] = 'B'; table[0 + i, 6, 0] = 'N'; table[0 + i, 7, 0] = 'R';
            }
            //To write the default state of the board.
            RefreshBoard(table);
            return table;
        }
        //Function for refreshing the board on screen
        static void RefreshBoard(char[,,] table)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (table[i, j, 1] == '-')
                        Console.ForegroundColor = ConsoleColor.Red;
                    else if (table[i, j, 1] == '+')
                        Console.ForegroundColor = ConsoleColor.Blue;
                    else
                        Console.ForegroundColor = ConsoleColor.Yellow;

                    Console.SetCursorPosition(3 * j + 3, 2 * i + 1);
                    Console.Write(table[i, j, 0]);
                }
            }
            Console.ForegroundColor = ConsoleColor.Gray;

            //CHECKING WHEN CASTLING PIECES MOVE BEFORE CASTLING
            //FOR BLUE KING
            if (table[7, 4, 0] == '.')
                firstMoveBlueK = true;
            //FOR RED KING
            if (table[0, 4, 0] == '.')
                firstMoveRedK = true;
            //FOR TOP LEFT ROOK
            if (table[0, 0, 0] == '.')
                firstMoveRedLR = true;
            //FOR TOP RIGHT ROOK
            if (table[0, 7, 0] == '.')
                firstMoveRedRR = true;
            //FOR BOTTOM LEFT ROOK
            if (table[7, 0, 0] == '.')
                firstMoveBlueLR = true;
            //FOR BOTTOM RIGHT ROOK
            if (table[7, 7, 0] == '.')
                firstMoveBlueRR = true;
        }
        //Controls if there are any opposion pawns near it when moved two squares
        static void enPassantPossible(char[,,] table,int lastPosX,int lastPosY,char opposing)
        {
            //Pawn on leftmost side
            if ((lastPosX - 3) / 3 == 0)
            {
                if (table[(lastPosY - 1) / 2, (lastPosX - 3) / 3 + 1, 0] == 'P' && table[(lastPosY - 1) / 2, (lastPosX - 3) / 3 + 1, 1] == opposing)
                    enPassant[(lastPosX - 3) / 3] = true;
            }
            //Pawn on rightmost side
            else if ((lastPosX - 3) / 3 == 7)
            {
                if (table[(lastPosY - 1) / 2, (lastPosX - 3) / 3 - 1, 0] == 'P' && table[(lastPosY - 1) / 2, (lastPosX - 3) / 3 + 1, 1] == opposing)
                    enPassant[(lastPosX - 3) / 3] = true;
            }
            //Pawns in between
            else
            {
                if ((table[(lastPosY - 1) / 2, (lastPosX - 3) / 3 - 1, 0] == 'P' && table[(lastPosY - 1) / 2, (lastPosX - 3) / 3 - 1, 1] == opposing) ||
                    (table[(lastPosY - 1) / 2, (lastPosX - 3) / 3 + 1, 0] == 'P' && table[(lastPosY - 1) / 2, (lastPosX - 3) / 3 + 1, 1] == opposing))
                    enPassant[(lastPosX - 3) / 3] = true;
            }
        }
        //Function that contains piece rules. Used for move validations on both play modes
        static bool MoveAccess(char[,,] table, char[] piece, int firstPosX, int firstPosY, int lastPosX, int lastPosY)
        {
            bool returnMove = false;
            char opposing;
            int pawnStart;
            shortCastleBlue = false;
            shortCastleRed = false;
            longCastleBlue = false;
            longCastleRed = false;
            promotion = false;
            //If destination isn't on the same player's piece
            if (piece[1] == '+' && table[(lastPosY - 1) / 2, (lastPosX - 3) / 3, 1] != '+' ||
                piece[1] == '-' && table[(lastPosY - 1) / 2, (lastPosX - 3) / 3, 1] != '-')
            {
                //FOR PAWN
                if (piece[0] == 'P')
                {
                    //Blue Pieces
                    if (piece[1] == '+')
                    {
                        opposing = '-';
                        pawnStart = 6;
                    }
                    //Red Pieces
                    else
                    {
                        opposing = '+';
                        pawnStart = 1;
                    }
                    //Forward moves
                    if (firstPosX == lastPosX && table[(lastPosY - 1) / 2, (lastPosX - 3) / 3, 0] == '.')
                    {
                        if (piece[1] == '+')
                        {
                            //Forward move
                            if (firstPosY - lastPosY == 2)
                            {
                                returnMove = true;
                                if ((lastPosY - 1) / 2 == 0 || (lastPosY - 1) / 2 == 7)
                                    promotion = true;
                            }

                            //Move two spaces
                            else if ((firstPosY - 1) / 2 == 6 && (lastPosY - 1) / 2 == pawnStart - 2 && table[((lastPosY - 1) / 2) + 1, (lastPosX - 3) / 3, 0] == '.')
                            {
                                returnMove = true;
                                enPassantPossible(table, lastPosX, lastPosY, opposing);
                            }
                        }
                        else
                        {
                            //Forward move
                            if (firstPosY - lastPosY == -2)
                            {
                                returnMove = true;
                                if ((lastPosY - 1) / 2 == 0 || (lastPosY - 1) / 2 == 7)
                                    promotion = true;
                            }

                            //Move two spaces
                            else if ((firstPosY - 1) / 2 == 1 && (lastPosY - 1) / 2 == pawnStart + 2 && table[((lastPosY - 1) / 2) - 1, (lastPosX - 3) / 3, 0] == '.')
                            {
                                returnMove = true;
                                enPassantPossible(table, lastPosX, lastPosY, opposing);
                            }
                        }
                    }
                    //Diagonal move(can be either capture or en passant)
                    else if ((firstPosX - lastPosX == 3 || firstPosX - lastPosX == -3) && (firstPosY - lastPosY == -2 || firstPosY - lastPosY == 2))
                    {
                        //Capture
                        if (table[(lastPosY - 1) / 2, (lastPosX - 3) / 3, 1] == opposing)
                        {
                            if (piece[1] == '+' && firstPosY - lastPosY == 2)
                            {
                                returnMove = true;
                                if ((lastPosY - 1) / 2 == 0 || (lastPosY - 1) / 2 == 7)
                                    promotion = true;
                            }

                            else if (piece[1] == '-' && firstPosY - lastPosY == -2)
                            {
                                returnMove = true;
                                if ((lastPosY - 1) / 2 == 0 || (lastPosY - 1) / 2 == 7)
                                    promotion = true;
                            }
                        }
                        //If en passant capture is available
                        else if (enPassant[(lastPosX - 3) / 3] && table[(lastPosY - 1) / 2, (lastPosX - 3) / 3, 1] == '*')
                        {
                            if (piece[1] == '+' && firstPosY - lastPosY == 2 && table[3, (lastPosX - 3) / 3, 0] == 'P' && table[3, (lastPosX - 3) / 3, 1] == opposing)
                            {
                                returnMove = true;
                                //For converting to notation
                                enPassantNotation = true;
                            }
                            else if (piece[1] == '-' && firstPosY - lastPosY == -2 && table[4, (lastPosX - 3) / 3, 0] == 'P' && table[4, (lastPosX - 3) / 3, 1] == opposing)
                            {
                                returnMove = true;
                                enPassantNotation = true;
                            }
                        }
                    }
                }
                //FOR KNIGHT
                else if (piece[0] == 'N')
                {
                    //Vertical L
                    if ((firstPosX - lastPosX == 3 || firstPosX - lastPosX == -3) &&
                        (firstPosY - lastPosY == 4 || firstPosY - lastPosY == -4))
                        returnMove = true;

                    //Horizontal L
                    else if ((firstPosX - lastPosX == 6 || firstPosX - lastPosX == -6) &&
                             (firstPosY - lastPosY == 2 || firstPosY - lastPosY == -2))
                        returnMove = true;
                }
                //FOR KING
                else if (piece[0] == 'K')
                {
                    //Can move only where is not threatened
                    if (piece[1] == '+' && redThreat[(lastPosY - 1) / 2, (lastPosX - 3) / 3] == false ||
                        piece[1] == '-' && blueThreat[(lastPosY - 1) / 2, (lastPosX - 3) / 3] == false)
                    {
                        //UP DOWN AND DIAGONAL
                        if (firstPosY - lastPosY == 2 || lastPosY - firstPosY == 2)
                        {
                            if (firstPosX - lastPosX == 3 || lastPosX - firstPosX == 3 || firstPosX - lastPosX == 0)
                                returnMove = true;
                        }
                        //LEFT RIGHT
                        else if (firstPosY - lastPosY == 0)
                        {
                            if (firstPosX - lastPosX == 3 || lastPosX - firstPosX == 3)
                                returnMove = true;
                        }
                    }
                }
                //FOR ROOK AND QUEEN (HORIZONTAL AND VERTICAL MOVES)
                else if ((piece[0] == 'R' || piece[0] == 'Q') && (firstPosX == lastPosX || firstPosY == lastPosY))
                {
                    //If no in between piece is found move will be true
                    returnMove = true;
                    //VERTICAL MOVEMENT
                    if (firstPosX == lastPosX)
                    {
                        //UPWARDS MOVE
                        if (lastPosY < firstPosY)
                        {
                            //LOOP CONTROLS IN BETWEEN PLACES
                            for (int i = 1; i <= ((firstPosY - 1) / 2) - ((lastPosY - 1) / 2) - 1; i++)
                            {
                                if (table[((firstPosY - 1) / 2) - i, (firstPosX - 3) / 3, 0] != '.')
                                {
                                    returnMove = false;
                                    break;
                                }
                            }
                        }
                        //DOWNWARDS MOVE
                        else
                        {
                            for (int i = 1; i <= ((lastPosY - 1) / 2) - ((firstPosY - 1) / 2) - 1; i++)
                            {
                                if (table[((firstPosY - 1) / 2) + i, (firstPosX - 3) / 3, 0] != '.')
                                {
                                    returnMove = false;
                                    break;
                                }
                            }
                        }
                    }
                    //HORIZONTAL MOVEMENT
                    else
                    {
                        //LEFT MOVE
                        if (lastPosX < firstPosX)
                        {
                            //LOOP CONTROLS IN BETWEEN PLACES
                            for (int i = 1; i <= ((firstPosX - 3) / 3) - ((lastPosX - 3) / 3) - 1; i++)
                            {
                                if (table[(firstPosY - 1) / 2, ((firstPosX - 3) / 3) - i, 0] != '.')
                                {
                                    returnMove = false;
                                    break;
                                }
                            }
                        }
                        //RIGHT MOVE
                        else
                        {
                            for (int i = 1; i <= ((lastPosX - 3) / 3) - ((firstPosX - 3) / 3) - 1; i++)
                            {
                                if (table[(firstPosY - 1) / 2, ((firstPosX - 3) / 3) + i, 0] != '.')
                                {
                                    returnMove = false;
                                    break;
                                }
                            }
                        }
                    }
                }
                //FOR BISHOP AND QUEEN (DIAGONAL MOVE)
                if ((piece[0] == 'B' || piece[0] == 'Q') && (double)Math.Abs(lastPosX - firstPosX) / Math.Abs(lastPosY - firstPosY) == 3.0 / 2)
                {
                    returnMove = true;
                    //UP
                    if (lastPosY < firstPosY)
                    {
                        //LEFT
                        if (lastPosX < firstPosX)
                        {
                            //LOOP CONTROLS IN BETWEEN PLACES
                            for (int i = 1; i <= ((firstPosX - 3) / 3) - ((lastPosX - 3) / 3) - 1; i++)
                            {
                                if (table[((firstPosY - 1) / 2) - i, ((firstPosX - 3) / 3) - i, 0] != '.')
                                {
                                    returnMove = false;
                                    break;
                                }
                            }
                        }
                        //RIGHT
                        else
                        {
                            //LOOP CONTROLS IN BETWEEN PLACES
                            for (int i = 1; i <= ((lastPosX - 3) / 3) - ((firstPosX - 3) / 3) - 1; i++)
                            {
                                if (table[((firstPosY - 1) / 2) - i, ((firstPosX - 3) / 3) + i, 0] != '.')
                                {
                                    returnMove = false;
                                    break;
                                }
                            }
                        }
                    }
                    //Down
                    else
                    {
                        //LEFT
                        if (lastPosX < firstPosX)
                        {
                            //LOOP CONTROLS IN BETWEEN PLACES
                            for (int i = 1; i <= ((firstPosX - 3) / 3) - ((lastPosX - 3) / 3) - 1; i++)
                            {
                                if (table[((firstPosY - 1) / 2) + i, ((firstPosX - 3) / 3) - i, 0] != '.')
                                {
                                    returnMove = false;
                                    break;
                                }
                            }
                        }
                        //RIGHT
                        else
                        {
                            //LOOP CONTROLS IN BETWEEN PLACES
                            for (int i = 1; i <= ((lastPosX - 3) / 3) - ((firstPosX - 3) / 3) - 1; i++)
                            {
                                if (table[((firstPosY - 1) / 2) + i, ((firstPosX - 3) / 3) + i, 0] != '.')
                                {
                                    returnMove = false;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            //Castling Rules
            else if (piece[0] == 'K' && !checkOnBlue && !checkOnRed && table[(lastPosY - 1) / 2, (lastPosX - 3) / 3, 0] == 'R' &&
                     table[(lastPosY - 1) / 2, (lastPosX - 3) / 3, 1] == piece[1])
            {
                 //Blue
                 if (piece[1] == '+')
                 {
                     //Castling short
                     if (!firstMoveBlueK && !firstMoveBlueRR && table[7, 5, 0] == '.' && table[7, 6, 0] == '.' &&
                        (lastPosX - 3) / 3 == 7 && !redThreat[7, 5] && !redThreat[7, 6])
                     {
                         shortCastleBlue = true;
                         returnMove = true;
                     }
                     //Castling long
                     else if (!firstMoveBlueK && !firstMoveBlueLR && table[7, 1, 0] == '.' && table[7, 2, 0] == '.' && table[7, 3, 0] == '.' &&
                             (lastPosX - 3) / 3 == 0 && !redThreat[7, 1] && !redThreat[7, 2] && !redThreat[7, 3])
                     {
                          longCastleBlue = true;
                          returnMove = true;
                     }
                 }
                 //Red
                 else
                 {
                    //Castling short
                    if (!firstMoveRedK && !firstMoveRedRR && table[0, 5, 0] == '.' && table[0, 6, 0] == '.' &&
                       (lastPosX - 3) / 3 == 7 && !blueThreat[0, 5] && !blueThreat[0, 6])
                    {
                        shortCastleRed = true;
                        returnMove = true;
                    }
                    //Castling long
                    else if (!firstMoveRedK && !firstMoveRedLR && table[7, 1, 0] == '.' && table[7, 2, 0] == '.' && table[7, 3, 0] == '.' &&
                            (lastPosX - 3) / 3 == 0 && !blueThreat[0, 1] && !blueThreat[0, 2] && !blueThreat[0, 3])
                    {
                        longCastleRed = true;
                        returnMove = true;
                    }
                }
            }
            return returnMove;
        }
        //All castlings are functions because ValidMoves function tries pieces at destinations for string mode and conversions and check controls
        //If change of values happen inside moveAccess function validMoves would make it without waiting for user inpur
        static void Castling(char[,,] table)
        {
            if (shortCastleBlue == true)
            {
                table[7, 6, 0] = 'K'; table[7, 6, 1] = '+';
                table[7, 5, 0] = 'R'; table[7, 5, 1] = '+';
                table[7, 4, 0] = '.'; table[7, 4, 1] = '*';
                table[7, 7, 0] = '.'; table[7, 7, 1] = '*';
            }
            else if (shortCastleRed == true)
            {
                table[0, 6, 0] = 'K'; table[0, 6, 1] = '-';
                table[0, 5, 0] = 'R'; table[0, 5, 1] = '-';
                table[0, 4, 0] = '.'; table[0, 4, 1] = '*';
                table[0, 7, 0] = '.'; table[0, 7, 1] = '*';
            }
            else if (longCastleBlue == true)
            {
                table[7, 2, 0] = 'K'; table[7, 2, 1] = '+';
                table[7, 3, 0] = 'R'; table[7, 3, 1] = '+';
                table[7, 4, 0] = '.'; table[7, 4, 1] = '*';
                table[7, 0, 0] = '.'; table[7, 0, 1] = '*';
            }
            else if(longCastleRed == true)
            {

                table[0, 2, 0] = 'K'; table[0, 2, 1] = '-';
                table[0, 3, 0] = 'R'; table[0, 3, 1] = '-';
                table[0, 4, 0] = '.'; table[0, 4, 1] = '*';
                table[0, 0, 0] = '.'; table[0, 0, 1] = '*';
            }
        }
        //Pawn Promotion
        //If called by cursor asks for input if input already written by string mode doesn't ask
        static char[] PawnPromote(char[] piece, string input)
        {
            int wrongTries = -1;
            if (input == "")
            {
                Console.SetCursorPosition(0, 27);
                Console.WriteLine("Which piece do you want to promote to?");
                Console.Write("(Queen, Knight, Rook or Bishop) : ");
                do
                {
                    input = Console.ReadLine().ToLower();
                    wrongTries++;
                } while (input != "queen" && input != "knight" && input != "rook" && input != "bishop");

                //Clean user prompt and inputs.
                StringClean(2 + wrongTries);
            }
            if (input == "queen") piece[0] = 'Q';
            else if (input == "knight") piece[0] = 'N';
            else if (input == "rook") piece[0] = 'R';
            else piece[0] = 'B';

            return piece;
        }
        //Valid moves to a given destination with a given piece, if 2 knight can move returns both of them
        //Used for string play mode and cursor to string conversions
        static char[,] ValidMoves(char[,,] table, char[] piece, int moveCoordX, int moveCoordY)
        {
            //Can be maximum 10 pieces of the same kind in a player
            char[,] validPieces = new char[10, 3];
            int firstPosX, firstPosY, pieceIndex = 0;
            bool access;

            for (int i = 0; i < table.GetLength(0); i++)
            {
                for (int j = 0; j < table.GetLength(1); j++)
                {
                    //Controls the pieces that have the same letter and the same color
                    if (table[i, j, 0] == piece[0] && table[i, j, 1] == piece[1])
                    {
                        //Position of the pieces
                        firstPosX = (3 * j) + 3;
                        firstPosY = (2 * i) + 1;

                        //Placement validation function
                        access = MoveAccess(table, piece, firstPosX, firstPosY, moveCoordX, moveCoordY);
                        //Stores all possible moves as characters
                        //Valid pieces array holds pieces' names with their indices on the board
                        if (access)
                        {
                            validPieces[pieceIndex, 0] = table[i, j, 0];
                            validPieces[pieceIndex, 1] = Convert.ToChar(j);
                            validPieces[pieceIndex, 2] = Convert.ToChar(i);
                            pieceIndex++;
                        }
                    }
                }
            }
            return validPieces;
        }
        //In 8x8 check array everwhere that is threatened by a player
        static void ThreatTable(char[,,] table)
        {
            char[] piece = new char[2];
            string[] bluePieces = { "R+", "N+", "B+", "Q+", "K+" };
            string[] redPieces = { "R-", "N-", "B-", "Q-", "K-" };

            char[,] validPieces;

            //Threat map is empited and refilled after
            for (int i = 0; i < redThreat.GetLength(0); i++)
            {
                for (int j = 0; j < redThreat.GetLength(1); j++)
                {
                    redThreat[i, j] = false;
                    blueThreat[i, j] = false;
                }
            }
            //Since our move validation is cursor based, loops are used as if cursor moves all the board
            for (int i = 1; i <= 15; i += 2)
            {
                for (int j = 3; j <= 24; j += 3)
                {
                    for (int k = 0; k < bluePieces.Length; k++)
                    {
                        piece[0] = bluePieces[k][0];
                        piece[1] = bluePieces[k][1];
                        validPieces = ValidMoves(table, piece, j, i);

                        if (validPieces[0, 0] != 0)
                            blueThreat[(i - 1) / 2, (j - 3) / 3] = true;

                        piece[0] = redPieces[k][0];
                        piece[1] = redPieces[k][1];
                        validPieces = ValidMoves(table, piece, j, i);

                        if (validPieces[0, 0] != 0)
                            redThreat[(i - 1) / 2, (j - 3) / 3] = true;
                    }
                }
            }
            //For pawns since threatening dont count as valid move if the space is empty same method above couldn't be used
            for (int i = 1; i <= 15; i += 2)
            {
                for (int j = 3; j <= 24; j += 3)
                {
                    if (table[(i - 1) / 2, (j - 3) / 3, 0] == 'P')
                    {
                        //Blue Pawn
                        if (table[(i - 1) / 2, (j - 3) / 3, 1] == '+')
                        {
                            if (i != 1 && i != 15)
                            {

                                if (j == 3)
                                    blueThreat[(i - 3) / 2, (j) / 3] = true;

                                else if (j == 24)
                                    blueThreat[(i - 3) / 2, (j - 6) / 3] = true;

                                else
                                {
                                    blueThreat[(i - 3) / 2, (j) / 3] = true;
                                    blueThreat[(i - 3) / 2, (j - 6) / 3] = true;
                                }
                            }
                        }
                        //Red Pawn
                        else
                        {
                            if (i != 1 && i != 15)
                            {

                                if (j == 3)
                                    redThreat[(i + 1) / 2, j / 3] = true;

                                else if (j == 24)
                                    redThreat[(i + 1) / 2, (j - 6) / 3] = true;

                                else
                                {
                                    redThreat[(i + 1) / 2, j / 3] = true;
                                    redThreat[(i + 1) / 2, (j - 6) / 3] = true;
                                }
                            }
                        }
                    }
                }
            }
            //Check is false until found below
            checkOnRed = false;
            checkOnBlue = false;
            //After updating threat tables we control if the king is under check
            for (int i = 0; i < table.GetLength(0); i++)
            {
                for (int j = 0; j < table.GetLength(1); j++)
                {
                    if (table[i, j, 0] == 'K' && table[i, j, 1] == '+' && redThreat[i, j] == true)
                        checkOnBlue = true;

                    else if (table[i, j, 0] == 'K' && table[i, j, 1] == '-' && blueThreat[i, j] == true)
                        checkOnRed = true;
                }
            }
        }
        //Controls the check situations after move
        static bool CheckSitutation(char[,,] table, char[] piece, int firstPosX, int firstPosY, int lastPosX, int lastPosY)
        {
            bool returnMove = true;
            bool checkOnRedBefore = checkOnRed;
            bool checkOnBlueBefore = checkOnBlue;
            //If king tries to capture a piece and that place is under threat controls would remove the piece while sending
            //the king back, this variable holds that piece
            char[] triedCaptureOn = new char[2];
            char[,,] alternateTable = table;
            //First makes the move and controls if the end result has checks still
            //Change taken index to default values in alternate table
            alternateTable[(firstPosY / 2), (firstPosX / 3 - 1), 0] = '.';
            alternateTable[(firstPosY / 2), (firstPosX / 3 - 1), 1] = '*';

            //Place piece to the new spot in alternate table
            for (int i = 0; i < 2; i++)
            {
                triedCaptureOn[i] = alternateTable[(lastPosY / 2), (lastPosX / 3 - 1), i];
                alternateTable[(lastPosY / 2), (lastPosX / 3 - 1), i] = piece[i];
            }

            ThreatTable(alternateTable);

            //If after move check still stays, move is invalid
            if (piece[1] == '+' && checkOnBlue == true)
                returnMove = false;

            else if (piece[1] == '-' && checkOnRed == true)
                returnMove = false;

            //Return the table back to it's original state to avoid complexity
            alternateTable[(lastPosY / 2), (lastPosX / 3 - 1), 0] = '.';
            alternateTable[(lastPosY / 2), (lastPosX / 3 - 1), 1] = '*';

            for (int i = 0; i < 2; i++)
            {
                alternateTable[(lastPosY / 2), (lastPosX / 3 - 1), i] = triedCaptureOn[i];
                alternateTable[(firstPosY / 2), (firstPosX / 3 - 1), i] = piece[i];
            }

            checkOnBlue = checkOnBlueBefore;
            checkOnRed = checkOnRedBefore;

            return returnMove;
        }
        //Converts cursor movements to string notations
        //Since we call this after move validation no validation done here
        static string CursorToString(char[,,]table, char[] piece, int movePosX, int movePosY, bool capture, string castlingNotation)
        {
            string move = "", moveX = "", moveY = "", firstPosX = "", firstPosY = "";
            char[,] validPieces = new char [10, 3];
            bool pawnPromotion = promotion;
         
            if (!shortCastleBlue && !shortCastleRed && !longCastleBlue && !longCastleRed)
            {
                validPieces = ValidMoves(table, piece, movePosX, movePosY);
                
                //Converts move coordinates to table positions such as a2, Nf3
                moveX = Convert.ToString(Convert.ToChar(97 + ((movePosX - 3) / 3)));
                moveY = Convert.ToString((17 - movePosY) / 2);

                //Converts first position coordinates to table positions for e in exd4 or b in Nbd4 
                firstPosX = Convert.ToString(Convert.ToChar(validPieces[0, 1] + 97));
                firstPosY = Convert.ToString(Convert.ToChar(validPieces[0, 2] + 1));
            }
            //Pawn move notations doesnt change based on number of available moves
            if (piece[0] == 'P')
            {
                if (capture)
                {
                    move = firstPosX + "x" + moveX + moveY;
                }
                else if (enPassantNotation)
                {
                    //exd6e.p.
                    move = firstPosX + "x" + moveX + moveY + "e.p.";
                }
                else
                {
                    move = moveX + moveY;
                }
            }
            //PAWN PROMOTE
            //exd8Q
            else if (pawnPromotion)
            {
                if (capture)
                    move = firstPosX + "x" + moveX + moveY + piece[0];

                else
                    move = moveX + moveY + piece[0];

                promotion = false;
            }
            //Castling notations
            else if (castlingNotation == "ShortCastle")
            {
                move = "O-O";
            }
            else if (castlingNotation == "LongCastle")
            {
                move = "O-O-O";
            }
            //If there is only one move found enters here
            else if (validPieces[1,0] == 0)
            {
                if (capture)
                    move = piece[0] + "x" + moveX + moveY;
                else
                    move = piece[0] + moveX + moveY;
            }
            //More than one move is available
            else
            {
                if (capture)
                {
                    //If two pieces were on the same column write moved piece's row.If not write their column
                    if (validPieces[0,1] == validPieces[1,1])
                        move = piece[0] + firstPosY + "x" + moveX + moveY;
                    else
                        move = piece[0] + firstPosX + "x" + moveX + moveY;
                }
                else
                {
                    //If two pieces were on the same column write moved piece's row.If not write their column
                    if (validPieces[0, 1] == validPieces[1, 1])
                        move = piece[0] + firstPosY + moveX + moveY;
                    else
                        move = piece[0] + firstPosX + moveX + moveY;
                }
            }
            return move;
        }
        //Writes moves on console screen. Used by all of the play modes
        //Counters array is for communicating between functions of how many moves have been done to change writing positions
        static int[] MovesTable(string move,char turn, int[] counters)
        {
            int moveNum = counters[0], placey = counters[1], placex = counters[2];
            //Write Move On Screen
            Console.ForegroundColor = ConsoleColor.Gray;
            //Change move's column after blue notation
            if (turn == '+')
            {
                //Every 15 consequtive move table slides to the right
                if (moveNum % 16 == 0)
                {
                    placex += 20;
                    placey = 1;
                }
                Console.SetCursorPosition(36 + placex, placey);
                Console.Write(moveNum + ". ");
            }
            //Change move's row after red notation
            else
            {
                Console.SetCursorPosition(47 + placex, placey);
                placey++;
                moveNum++;
            }
            Console.Write(move + " ");

            counters[0] = moveNum;
            counters[1] = placey;
            counters[2] = placex;
            return counters;
        }
        //Saves the game into a text file
        static void SaveMoves(string[] moves)
        {
            StreamWriter game = File.CreateText("Previous_Game.txt");
            for (int i = 0; i < moves.Length / 2; i += 2)
            {
                if (moves[i] != "")
                {
                    for (int j = 0; j < 2; j++)
                    {
                        game.Write(moves[i + j] + " ");
                    }
                    game.WriteLine();
                }
            }
            game.Close();
        }
        //Read game from file for demo mode
        static string[] ReadMoves(string path)
        {
            int moveLines = 0,moveCount = 0;
            //Consequtive 2 moves
            string[] consMoves;
            //First gets the file length and move count
            StreamReader readLines = File.OpenText(path);
            do
            {
                consMoves = readLines.ReadLine().Split();
                for (int i = 0; i < consMoves.Length; i++)
                {
                    if (consMoves[i] != "")
                        moveCount ++;
                }
                moveLines++;
            } while (!readLines.EndOfStream);
            readLines.Close();

            //Creates moves array based on the move count and fills it in
            StreamReader game = File.OpenText(path);
            string[] moves = new string[moveCount];
            for (int i = 0; i < moveLines * 2; i += 2)
            {
                consMoves = game.ReadLine().Split();
                //It can have 2 or 1 moves
                for (int j = 0; j < consMoves.Length; j++)
                {
                    if (consMoves[j] != "")
                        moves[i + j] = consMoves[j];
                }
            }
            game.Close();

            return moves;
        }
        //Gives capture hints to user
        static void Hint(char[,,] table, char turn)
        {
            string move = "";
            char[,] validmove = null;
            char[] pieces = {'P','R', 'B', 'N', 'Q', 'K'};
            char[] piece = new char[2];
            //Blue turn
            if (turn == '+')
            {
                piece[1] = '+';
                //Iterate over the board
                for (int i = 0; i < blueThreat.GetLength(0); i++)
                {
                    for (int j = 0; j < blueThreat.GetLength(1); j++)
                    {
                        //If blue threats a red piece
                        if (table[i,j,1] == '-' && blueThreat[i,j])
                        {
                            //Find the piece that threatens the spot
                            for (int k = 0; k < pieces.Length; k++)
                            {
                                piece[0] = pieces[k];
                                validmove = ValidMoves(table,piece,3 * j + 3, 2 * i + 1);
                                if (validmove[0,0] != 0)
                                    break;
                            }
                            //Convert it to string notation
                            move += CursorToString(table,piece,3 * j + 3, 2 * i + 1,true,"") + " ";
                        }
                    }
                }
            }
            //Red turn
            else
            {
                piece[1] = '-';
                //Iterate over the board
                for (int i = 0; i < blueThreat.GetLength(0); i++)
                {
                    for (int j = 0; j < blueThreat.GetLength(1); j++)
                    {
                        //If red threatens a blue piece
                        if (table[i, j, 1] == '+' && redThreat[i, j])
                        {
                            //Find the piece that threatens the spot
                            for (int k = 0; k < pieces.Length; k++)
                            {
                                piece[0] = pieces[k];
                                validmove = ValidMoves(table, piece, 3 * j + 3, 2 * i + 1);
                                if (validmove[0, 0] != 0)
                                    break;
                            }
                            //Convert it to string notation
                            move += CursorToString(table, piece, 3 * j + 3, 2 * i + 1, true, "") + " ";
                        }
                    }
                }
            }
            Console.SetCursorPosition(0, 27);
            Console.ForegroundColor = ConsoleColor.Gray;
            if (move != "")
                Console.WriteLine("Hint: " + move);
            else
                Console.WriteLine("No capture available.");
        }
        //Call if check on blue or check on red
        //End game == true means game is over
        static bool EndGame(char[,,] table, char turn, int[]positionofThePieceXY)
        {
            bool endgame = true;
            char[] pieces = new char[16];
            char[] piece = new char[2];
            char[,] validmoves;
            piece[1] = turn;

            int pieceIndex = 0;

            int[] positionOfKingXY = new int[2];

            //Find Player's Pieces
            for (int i = 0; i < table.GetLength(0); i++)
            {
                for (int j = 0; j < table.GetLength(1); j++)
                {
                    if (table[i, j, 1] == turn)
                    {
                        if (table[i, j, 0] != 'K')
                        {
                            pieces[pieceIndex] = table[i, j, 0];
                            pieceIndex++;
                        }
                        else if (table[i,j,0] == 'K')
                        {
                            positionOfKingXY[0] = j;
                            positionOfKingXY[1] = i;

                            pieceIndex++;
                        }
                    }
                }
            }
            //Check Piece On the same Row
            if (positionOfKingXY[1] == positionofThePieceXY[1])
            {
                //From right
                if (positionOfKingXY[0] < positionofThePieceXY[0])
                {
                    for (int i = positionOfKingXY[0]; i < positionofThePieceXY[0]; i++)
                    {
                        for (int j = 0; j < pieces.Length; j++)
                        {
                            piece[0] = pieces[j];
                            validmoves = ValidMoves(table, piece, 3 * j + 3, 2 * positionOfKingXY[1] + 1);
                            if (validmoves[0, 0] != 0)
                            {
                                endgame = false;
                                break;
                            }
                        }
                        if (endgame == false)
                            break;
                    }
                }
                //Check From right
                else
                {
                    for (int i = positionofThePieceXY[0]; i < positionOfKingXY[0]; i++)
                    {
                        for (int j = 0; j < pieces.Length; j++)
                        {
                            piece[0] = pieces[j];
                            validmoves = ValidMoves(table, piece, 3 * j + 3, 2 * positionOfKingXY[1] + 1);
                            if (validmoves[0, 0] != 0)
                            {
                                endgame = false;
                                break;
                            }
                        }
                        if (endgame == false)
                            break;
                    }
                }
            }
            //Check On the same column
            else if (positionOfKingXY[0] == positionofThePieceXY[0])
            {
                //From up
                if (positionofThePieceXY[1] < positionOfKingXY[1])
                {
                    for (int i = positionofThePieceXY[1]; i < positionOfKingXY[1]; i++)
                    {
                        for (int j = 0; j < pieces.Length; j++)
                        {
                            piece[0] = pieces[j];
                            validmoves = ValidMoves(table, piece, 3 * positionOfKingXY[0] + 3, 2 * i + 1);
                            if (validmoves[0, 0] != 0)
                            {
                                endgame = false;
                                break;
                            }
                        }
                        if (endgame == false)
                            break;
                    }
                }
                //From down
                else
                {
                    for (int i = positionOfKingXY[1]; i < positionofThePieceXY[1]; i++)
                    {
                        for (int j = 0; j < pieces.Length; j++)
                        {
                            piece[0] = pieces[j];
                            validmoves = ValidMoves(table, piece, 3 * positionOfKingXY[0] + 3, 2 * i + 1);
                            if (validmoves[0, 0] != 0)
                            {
                                endgame = false;
                                break;
                            }
                        }
                        if (endgame == false)
                            break;
                    }
                }
            }
            //Check Piece on Diagonal Space
            else if((double)Math.Abs((3 * positionofThePieceXY[0] + 3) - (3 * positionOfKingXY[0] + 3)) /
                            Math.Abs((2 * positionofThePieceXY[1] - 1) - (2 * positionOfKingXY[1] - 1)) == 3.0 / 2)
            {
                //From up
                if (positionofThePieceXY[1] < positionOfKingXY[1])
                {
                    //From right
                    if (positionofThePieceXY[0] > positionOfKingXY[0])
                    {
                        for (int i = positionOfKingXY[1]; i < positionofThePieceXY[1]; i--)
                        {
                            for (int j = positionOfKingXY[0]; j < positionofThePieceXY[0]; j++)
                            {
                                if (i == j)
                                {
                                    for (int k = 0; k < pieces.Length; k++)
                                    {
                                        piece[0] = pieces[k];
                                        validmoves = ValidMoves(table, piece, 3 * j + 3, 2 * i + 1);
                                        if (validmoves[0, 0] != 0)
                                        {
                                            endgame = false;
                                            break;
                                        }
                                    }
                                }
                                if (endgame == false)
                                    break;
                            }
                            if (endgame == false)
                                break;
                        }
                    }
                    //From left
                    else
                    {
                        for (int i = positionOfKingXY[1]; i < positionofThePieceXY[1]; i--)
                        {
                            for (int j = positionOfKingXY[0]; j < positionofThePieceXY[0]; j--)
                            {
                                if (i == j)
                                {
                                    for (int k = 0; k < pieces.Length; k++)
                                    {
                                        piece[0] = pieces[k];
                                        validmoves = ValidMoves(table, piece, 3 * j + 3, 2 * i + 1);
                                        if (validmoves[0, 0] != 0)
                                        {
                                            endgame = false;
                                            break;
                                        }
                                    }
                                }
                                if (endgame == false)
                                    break;
                            }
                            if (endgame == false)
                                break;
                        }
                    }
                }
                //From down
                else
                {
                    //From right
                    if (positionofThePieceXY[0] > positionOfKingXY[0])
                    {
                        for (int i = positionOfKingXY[1]; i < positionofThePieceXY[1]; i++)
                        {
                            for (int j = positionOfKingXY[0]; j < positionofThePieceXY[0]; j++)
                            {
                                if (i == j)
                                {
                                    for (int k = 0; k < pieces.Length; k++)
                                    {
                                        piece[0] = pieces[k];
                                        validmoves = ValidMoves(table, piece, 3 * j + 3, 2 * i + 1);
                                        if (validmoves[0, 0] != 0)
                                        {
                                            endgame = false;
                                            break;
                                        }
                                    }
                                }
                                if (endgame == false)
                                    break;
                            }
                            if (endgame == false)
                                break;
                        }
                    }
                    //From left
                    else
                    {
                        for (int i = positionOfKingXY[1]; i < positionofThePieceXY[1]; i++)
                        {
                            for (int j = positionOfKingXY[0]; j < positionofThePieceXY[0]; j--)
                            {
                                if (i == j)
                                {
                                    for (int k = 0; k < pieces.Length; k++)
                                    {
                                        piece[0] = pieces[k];
                                        validmoves = ValidMoves(table, piece, 3 * j + 3, 2 * i + 1);
                                        if (validmoves[0, 0] != 0)
                                        {
                                            endgame = false;
                                            break;
                                        }
                                    }
                                }
                                if (endgame == false)
                                    break;
                            }
                            if (endgame == false)
                                break;
                        }
                    }
                }
            }
            //Also control if king can move out of the way
            for (int i = 0; i < table.GetLength(0); i++)
            {
                for (int j = 0; j < table.GetLength(1); j++)
                {
                    piece[0] = 'K';
                    validmoves = ValidMoves(table, piece, 3 * j + 3, 2 * i + 1);
                    if (validmoves[0, 0] != 0)
                    {
                        endgame = false;
                        break;
                    }
                }
                if (endgame == false)
                    break;
            }
            return endgame;
        }
        static bool Draw(char[,,] table, char turn)
        {
            char[] pieces = new char[16];
            char[] piece = new char[2];
            char[,] validmoves;
            piece[1] = turn;
            int pieceIndex = 0;

            bool draw = true;
            //Find Player's Pieces
            for (int i = 0; i < table.GetLength(0); i++)
            {
                for (int j = 0; j < table.GetLength(1); j++)
                {
                    if (table[i, j, 1] == turn)
                    {
                        pieces[pieceIndex] = table[i, j, 0];
                        pieceIndex++;
                    }
                }
            }
            for (int i = 0; i < pieces.Length; i++)
            {
                piece[0] = pieces[i];
                for (int j = 0; j < 8; j++)
                {
                    for (int k = 0; k < 8; k++)
                    {
                        validmoves = ValidMoves(table, piece, 3 * k + 3, 2 * j + 1);
                        if (validmoves[0, 0] != 0)
                        {
                            draw = false;
                            break;
                        }
                    }
                    if (draw == false)
                        break;
                }
                if (draw == false)
                    break;
            }
            return draw;
        }
        //Cursor PlayMode
        //moveCount shift and placeY parameters are to keep going from the move table's place demoMode left.
        //turn parameter is to keep count of whose turn was it after demo mode,moveIndex is to keep saving moves after demo mode to cursor mode change
        static void CursorPlayMode(char[,,] table,int moveCount, int shift, int placeY, char turn, int movesIndex, string[] moves)
        {
            int cursorx = 12, cursory = 7, firstPosY = 0, firstPosX = 0;
            bool movePiece = false, pieceOnHand = false, access, checkControl;
            char[] piece = new char[2];
            string move, castlingNotation;
            bool endgame,draw;
            int[] counters = new int[3];
            int[] positionofThePieceXY = new int[2];
            counters[0] = moveCount;
            counters[1] = placeY;
            counters[2] = shift;

            ConsoleKeyInfo cki;
            while (true)
            {
                Console.SetCursorPosition(cursorx, cursory);
                //Move Cursor Inside Board
                cki = Console.ReadKey();
                if (cki.Key == ConsoleKey.RightArrow && cursorx < 24) cursorx += 3;
                if (cki.Key == ConsoleKey.LeftArrow && cursorx > 3) cursorx -= 3;
                if (cki.Key == ConsoleKey.UpArrow && cursory > 1) cursory -= 2;
                if (cki.Key == ConsoleKey.DownArrow && cursory < 15) cursory += 2;

                RefreshBoard(table);

                if (cki.Key == ConsoleKey.H)
                    Hint(table,turn);

                //Save Moves
                else if (cki.Key == ConsoleKey.Spacebar)
                {
                    SaveMoves(moves);
                    Console.SetCursorPosition(0, 27);
                    Console.WriteLine("Game is saved to the source code path in the name of Previous_Game.txt.\nPress Space again to exit or any key to keep playing.");
                    cki = Console.ReadKey();
                    if (cki.Key == ConsoleKey.Spacebar)
                        break;
                    else
                        StringClean(2);
                }
                //Move Piece
                else if (cki.Key == ConsoleKey.Enter)
                {
                    StringClean(2);
                    //Take piece
                    if (pieceOnHand == false && table[(cursory / 2), (cursorx / 3 - 1), 0] != '.' && turn == table[(cursory / 2), (cursorx / 3 - 1), 1])
                    {
                        //Exchange values
                        for (int i = 0; i < 2; i++)
                            piece[i] = table[(cursory / 2), (cursorx / 3 - 1), i];
                        
                        //Save first position for move controls
                        firstPosY = cursory;
                        firstPosX = cursorx;

                        pieceOnHand = true;
                        movePiece = true;
                    }
                    //Place piece
                    else if (pieceOnHand == true)
                    {
                        //Controls if piece is moved somewhere 
                        if (cursory != firstPosY || cursorx != firstPosX)
                        {
                            //Valid move control
                            access = MoveAccess(table, piece, firstPosX, firstPosY, cursorx, cursory);
                            if (access)
                            {
                                castlingNotation = "";
                                if (promotion)
                                    PawnPromote(piece , "");

                                else if (shortCastleBlue || shortCastleRed || longCastleBlue || longCastleRed)
                                {
                                    Castling(table);
                                    //For notation
                                    if (shortCastleBlue || shortCastleRed)
                                    {
                                        castlingNotation = "ShortCastle";
                                    }
                                    else
                                    {
                                        castlingNotation = "LongCastle";
                                    }
                                }

                                //Check control
                                if (!shortCastleBlue && !shortCastleRed && !longCastleBlue && !longCastleRed)
                                    checkControl = CheckSitutation(table, piece, firstPosX, firstPosY, cursorx, cursory);
                                //Castling controls its own check
                                else
                                    checkControl = true;

                                if (checkControl)
                                {
                                    //Capture bool is for writing the move on screen
                                    if (table[(cursory / 2), (cursorx / 3 - 1), 0] != '.') capture = true;
                                    else capture = false;

                                    //Write move on screen
                                    move = CursorToString(table, piece, cursorx, cursory, capture, castlingNotation);

                                    if (!shortCastleBlue && !shortCastleRed && !longCastleBlue && !longCastleRed)
                                    {
                                        if (enPassantNotation)
                                        {
                                            if (turn == '+')
                                            {
                                                table[3, (cursorx - 3) / 3, 0] = '.';
                                                table[3, (cursorx - 3) / 3, 1] = '*';
                                            }
                                            else
                                            {
                                                table[4, (cursorx - 3) / 3, 0] = '.';
                                                table[4, (cursorx - 3) / 3, 1] = '*';
                                            }
                                            enPassant[(cursorx - 3) / 3] = false;
                                            enPassantNotation = false;
                                        }
                                        //Change taken index to default values
                                        table[(firstPosY / 2), (firstPosX / 3 - 1), 0] = '.';
                                        table[(firstPosY / 2), (firstPosX / 3 - 1), 1] = '*';

                                        //Place piece to the new spot
                                        for (int i = 0; i < 2; i++)
                                            table[(cursory / 2), (cursorx / 3 - 1), i] = piece[i];
                                    }
                                    //New state of threats on the board
                                    ThreatTable(table);

                                    if (checkOnBlue || checkOnRed)
                                        move += "+";

                                    moves[movesIndex] = move;
                                    movesIndex++;
                                    counters = MovesTable(move, turn, counters);


                                    //Change Turns After Placement
                                    if (turn == '+') turn = '-';
                                    else if (turn == '-') turn = '+';

                                    endgame = false;
                                    if (checkOnBlue || checkOnRed)
                                    {
                                        positionofThePieceXY[0] = cursorx / 3 - 1;
                                        positionofThePieceXY[1] = cursory / 2;

                                        endgame = EndGame(table, turn, positionofThePieceXY);
                                        if (turn == '+')
                                        {
                                            if (!blueThreat[(cursory / 2), (cursorx / 3 - 1)] || endgame)
                                                endgame = true;
                                        }
                                        else
                                        {
                                            if (!redThreat[(cursory / 2), (cursorx / 3 - 1)] || endgame)
                                                endgame = true;
                                        }
                                    }
                                    draw = Draw(table, turn);
                                    if (endgame == true || draw == true)
                                    {
                                        RefreshBoard(table);
                                        Console.SetCursorPosition(0,27);
                                        Console.WriteLine("Game Over.Press Space to save the game or anything else to exit.");
                                        cki = Console.ReadKey();
                                        //Save Moves
                                        if (cki.Key == ConsoleKey.Spacebar)
                                        {
                                            SaveMoves(moves);
                                            Console.SetCursorPosition(0, 27);
                                            Console.WriteLine("Game is saved to the source code path in the name of Previous_Game.txt.");
                                        }
                                        break;
                                    }
                                }
                            }
                            //Replace the piece where it was taken
                            else
                            {
                                for (int i = 0; i < 2; i++)
                                    table[(firstPosY / 2), (firstPosX / 3 - 1), i] = piece[i];
                                cursorx = firstPosX;
                                cursory = firstPosY;
                            }
                        }
                        //If piece is placed back where it was taken, calculations aren't made and turn doesn't skip
                        else
                            for (int i = 0; i < 2; i++)
                                table[(cursory / 2), (cursorx / 3 - 1), i] = piece[i];

                        pieceOnHand = false;
                        movePiece = false;
                    }
                    RefreshBoard(table);
                }
                //If piece is taken, writes it wherever cursor is on the screen and holds the taken place empty for now doesn't clean immadiately
                if (movePiece == true)
                {
                    pieceOnHand = true;
                    if (piece[1] == '-')
                        Console.ForegroundColor = ConsoleColor.Red;
                    else
                        Console.ForegroundColor = ConsoleColor.Blue;

                    Console.SetCursorPosition(cursorx, cursory);
                    Console.Write(piece[0]);
                    Console.SetCursorPosition(firstPosX, firstPosY);
                    Console.Write('.');
                }
            }
        }
        //Function that cleans user prompts or inputs from screen
        static void StringClean(int lines) 
        {
            Console.SetCursorPosition(0, 27);
            for (int i = 0; i < lines; i++)
            {
                for (int j = 0; j < 120; j++)
                    Console.Write(" ");
            }
            Console.SetCursorPosition(0, 27);
        }
        //String PlayMode
        static void StringPlayMode(char[,,] table,bool demoMode, string[] preMove, string[] moves)
        {
            string move, newPlayMode;
            bool valid, input, syntaxControl,exitGame = false,checkControl,castle;
            char promotion,checkNot;
            int clarify, moveCount = 1, shift = 0, cursorPosX = 0, cursorPosY = 1, placey = 1, subcontrol, demoModeMove = 0, movesIndex = 0;
            int[] movePos = new int[2];
            int wrongTries = 0;
            char coordX = ' ', coordY = ' ', turn = '+', multiControl;
            char[] piece = new char[2];
            char[,] validPieces;
            char[] landedOnPiece = new char[2];
            bool endgame, draw;
            int[] positionofThePieceXY = new int[2];
            char[,,] temptable;
            int[] counters = new int[3];
            counters[0] = moveCount;
            counters[1] = placey;
            counters[2] = shift;

            ConsoleKeyInfo cki;
            //Main Game Loop
            while (true)
            {
                valid = false;
                syntaxControl = false;
                piece[1] = turn;
                multiControl = ' ';
                clarify = 0;
                subcontrol = 0;
                promotion = ' ';
                checkNot = ' ';
                castle = false;
                //Loops until a syntatical correct move is written
                do {
                    enPassantNotation = false;
                    input = true;
                    move = "";
                    //String Input Play
                    if (demoMode == false)
                    {
                        //Color of the piece user will move is shown while writing
                        if (turn == '+') Console.ForegroundColor = ConsoleColor.Blue;
                        else Console.ForegroundColor = ConsoleColor.Red;

                        Console.SetCursorPosition(cursorPosX, 18 + cursorPosY);
                        move = Console.ReadLine();

                        StringClean(2);
                        //Every 8 input entered cursor position shifts to the right
                        if (cursorPosY == 8)
                        {
                            cursorPosY = 1;
                            cursorPosX += 8;
                        }
                        else
                            cursorPosY++;
                    }
                    //DemoMode Play
                    else
                    {
                        //Gets inputs either space or enter
                        Console.ForegroundColor = ConsoleColor.Gray;

                        //Clear previous sentences
                        StringClean(2);

                        //Control if the next move is available
                        if (demoModeMove < preMove.Length)
                            Console.Write("Please press \"Space\" to play the next move or \"Enter\" to exit demo mode.\nNext move is: " + preMove[demoModeMove]);
                        else
                            Console.Write("Maximum amount of moves has been reached.\nPlease press any key to continue.");
                        do
                        {
                            cki = Console.ReadKey();
                        } while (cki.Key != ConsoleKey.Spacebar && cki.Key != ConsoleKey.Enter);

                        //If enter is pressed or maximum amount of numbers are played player is asked to switch modes
                        if (cki.Key == ConsoleKey.Enter || demoModeMove == preMove.Length)
                        {
                            StringClean(2);
                            //Get the new play mode user wants
                            Console.Write("Please continue with either string or cursor playmodes: \"string\" or \"cursor\": ");
                            do
                            {
                                newPlayMode = Console.ReadLine().ToLower();
                                wrongTries++;
                            } while (newPlayMode != "string" && newPlayMode != "cursor");
                            StringClean(wrongTries + 2);

                            //String play mode will keep looping while taking user inputs as moves
                            if (newPlayMode == "string")
                                demoMode = false;
                            else
                                CursorPlayMode(table, counters[0], counters[2], counters[1], turn, movesIndex, moves);
                        }
                        else
                        {
                            move = preMove[demoModeMove];
                            demoModeMove++;
                        }
                    }
                    //Save the game
                    if (move == "save")
                    {
                        SaveMoves(moves);
                        Console.SetCursorPosition(0, 27);
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.Write("Game is saved to the source code path in the name of Previous_Game.txt.\nWrite yes to continue playing or anything to exit the game: ");
                        move = Console.ReadLine();
                        if (move == "yes")
                        {
                            StringClean(2);
                            Console.SetCursorPosition(cursorPosX, 18 + cursorPosY);

                            if (turn == '+') Console.ForegroundColor = ConsoleColor.Blue;
                            else Console.ForegroundColor = ConsoleColor.Red;
                            move = Console.ReadLine();
                        }
                        else
                        {
                            exitGame = true;
                            break;
                        }
                    }
                    //Capture hint
                    if (move == "hint")
                    {
                        Hint(table, turn);
                        input = false;
                    }
                    //Pawn movement(e4)
                    else if (move.Length == 2)
                    {
                        piece[0] = 'P';
                        coordX = move[0];
                        coordY = move[1];
                        //Promotion notation should have been entered
                        if (move[1] == '8' || move[1] == '1')
                            input = false;
                    }
                    //Other pieces' simple movements or kingside castling (Nf3 or O-O or e8N)
                    else if (move.Length == 3)
                    {
                        if (move[2] == 'N' || move[2] == 'B' || move[2] == 'Q' || move[2] == 'R')
                        {
                            piece[0] = 'P';
                            coordX = move[0];
                            coordY = move[1];
                            promotion = move[2];
                            if (move[1] != '8' && move[1] != '1')
                                input = false;
                        }
                        else if (move == "O-O")
                        {
                            piece[0] = 'K';
                            if (turn == '+')
                            {
                                coordX = 'h';
                                coordY = '1';
                            }
                            else
                            {
                                coordX = 'h';
                                coordY = '8';
                            }
                            castle = true;
                            break;
                        }
                        else
                        {
                            piece[0] = move[0];
                            coordX = move[1];
                            coordY = move[2];
                        }
                    }
                    //exd4 or Nbd4 or Nxd4 or Qe3+
                    else if (move.Length == 4)
                    {
                        //In other pieces than pawns, deciding factor if capture notation or not is second letter (Nxd4 Nbd2)
                        if (move[0] == 'R' || move[0] == 'N' || move[0] == 'B' || move[0] == 'Q' || move[0] == 'K')
                        {
                            if (move[3] == '+')
                            {
                                piece[0] = move[0];
                                coordX = move[1];
                                coordY = move[2];
                                checkNot = '+';
                            }
                            else
                            {
                                piece[0] = move[0];
                                coordX = move[2];
                                coordY = move[3];
                                if (move[1] != 'x')
                                    multiControl = move[1];
                            }
                        }
                        //Pawn's capture notation(exd4)
                        else
                        {
                            coordX = move[2];
                            coordY = move[3];
                            multiControl = move[0];

                            if (move[1] != 'x' || move[3] == '8' || move[3] == '1') 
                                input = false;
                        }
                    }
                    //Capture clarifications of pieces other than pawn (N1xf3 Nbxd2) or promotion capture of pawn (dxe8N) (Qxd4+)
                    else if (move.Length == 5)
                    {
                        //Promotion Capture
                        if (move[4] == 'N' || move[4] == 'B' || move[4] == 'Q' || move[4] == 'R')
                        {
                            coordX = move[2];
                            coordY = move[3];
                            promotion = move[4];
                            if (move[1] != 'x' || (coordY != '1' && coordY != '8')) input = false;

                        }
                        //Capture check
                        else if (move[4] == '+')
                        {
                            piece[0] = move[0];
                            coordX = move[2];
                            coordY = move[3];
                            checkNot = '+';
                            if (move[1] != 'x') input = false;
                        }
                        //Clarification notation
                        else
                        {
                            piece[0] = move[0];
                            multiControl = move[1];
                            coordX = move[3];
                            coordY = move[4];
                            if (move[2] != 'x') input = false;
                        }
                    }

                    //Queenside Castling  O-O-O
                    else if (move.Length == 6)
                    {
                        if (move == "O-O-O")
                        {
                            piece[0] = 'K';
                            if (turn == '+')
                            {
                                coordX = 'a';
                                coordY = '1';
                            }
                            else
                            {
                                coordX = 'a';
                                coordY = '8';
                            }
                            castle = true;
                            break;
                        }  
                    }
                    //en passant exd6e.p.
                    else if (move.Length == 8 && move.Substring(4) == "e.p.")
                    {
                        piece[0] = 'P';
                        coordX = move[2];
                        coordY = move[3];
                        enPassantNotation = true;
                        if (move[1] != 'x') input = false;
                    }
                    //Cant be a move
                    else
                        input = false;
                    //Input validations. If previous requirements are met continues looking for the next validation
                    if (input)
                    {
                        input = false;
                        //Move Coordinate X must be between letters a and h
                        for (int i = 0; i < 8; i++)
                            if (coordX == Convert.ToChar(97 + i))
                            { input = true; break; }
                        if (input)
                        {
                            input = false;
                            //Move Coordinate Y must be between 1 and 8
                            for (int i = 0; i < 8; i++)
                                if (coordY == Convert.ToChar(Convert.ToString(i + 1)))
                                { input = true; break; }

                            //Multiple Move Clarifications: must be either between a and h or 1 and 8
                            if (input && multiControl != ' ')
                            {
                                input = false;
                                for (int i = 0; i < 8; i++)
                                    if (multiControl != Convert.ToChar(97 + i) || multiControl != Convert.ToChar(Convert.ToString(i + 1)))
                                    { input = true; break; }
                            }
                        }
                    }
                    if (!input && move != "")
                    {
                        if (move != "hint")
                        {
                            Console.ForegroundColor = ConsoleColor.Gray;
                            Console.SetCursorPosition(0, 27);
                            Console.WriteLine("Invalid move");
                        }
                    }
                } while (!input);
                if (exitGame == true)
                    break;
                //Clears notifications written below screen
                StringClean(2);

                //Destination coordinates are turned into console coordinates to use MoveAccess function that cursor movement uses
                //(a, b, c... will be converted to corresponding coordinate X on console screen 3, 6, 9..)
                if (coordX == 'a' || coordX == 'b' || coordX == 'c' || coordX == 'd' || coordX == 'e' || coordX == 'f' || coordX == 'g' || coordX == 'h')
                    movePos[0] = 3 * (Convert.ToInt32(coordX) % 97) + 3;
                //(1, 2, 3....  will be converted to  corresponding coordinate Y on console screen  15, 13, 11, ...)
                if (coordY == '1' || coordY == '2' || coordY == '3' || coordY == '4' || coordY == '5' || coordY == '6' || coordY == '7' || coordY == '8')
                    movePos[1] = 17 - 2 * Convert.ToInt32(Convert.ToString(coordY));

                //We control if capture notation is written where it is able to capture with the above coordinates because
                //valid move control doesn't differentiate between capture and normal move, both would interchange.
                //False if the move contains x and capture is not possible 
                if (enPassantNotation == false && castle == false)
                {
                    for (int i = 0; i < move.Length; i++)
                    {
                        if (table[(movePos[1] / 2), (movePos[0] / 3 - 1), 0] != '.' && move[i] == 'x')
                        {
                            syntaxControl = true;
                            break;
                        }
                        else if (table[(movePos[1] / 2), (movePos[0] / 3 - 1), 0] == '.' && move[i] != 'x')
                            subcontrol++;
                    }
                    if (subcontrol == move.Length)
                        syntaxControl = true;
                }
                else
                    syntaxControl = true;

                Console.ForegroundColor = ConsoleColor.Gray;
                Console.SetCursorPosition(0, 27);
                if (syntaxControl)
                {
                    //Get the piece or pieces which have a valid move if not an empty array
                    validPieces = ValidMoves(table, piece, movePos[0], movePos[1]);
                    //If there is at least one piece move is valid(Can turn false later if move clarification was not enough)
                    if (validPieces[0, 0] != 0)
                        valid = true;
                    //Enters here when more than one piece can move to the position and a string with clarification is written by the user
                    if (validPieces[1,0] != 0 && multiControl != ' ')
                    {
                        //This control means a letter is entered so we are looking at columns
                        if (Convert.ToInt32(multiControl) > 8)
                        {
                            //Convert the letter to an index and find the piece that has the given coordinate in its column
                            multiControl = Convert.ToChar(Convert.ToInt32(multiControl) % 97);
                            for (int i = 0; i < validPieces.GetLength(0); i++)
                            {
                                if (validPieces[i, 0] != 0 && validPieces[i, 1] == multiControl)
                                {
                                    //Find the piece on column
                                    for (int j = 0; j < table.GetLength(0); j++)
                                    {
                                        if (table[j, multiControl, 0] == validPieces[i, 0] && table[j, multiControl, 1] == turn)
                                        {
                                            //Valid piece is attained to the first index of the validpieces
                                            for (int k = 0; k < 2; k++)
                                                validPieces[0, k + 1] = validPieces[i, k + 1];
                                            clarify++;
                                        }
                                    }
                                }
                            }
                        }
                        //If a row is entered insted of column such as N1xd4, finds the valid piece with the given row
                        else
                        {
                            //Convert the number to index by lowering it 1
                            multiControl = Convert.ToChar(Convert.ToInt32(multiControl) - 1);
                            for (int i = 0; i < validPieces.GetLength(0); i++)
                            {
                                if (validPieces[i, 0] != 0 && validPieces[i, 2] == multiControl)
                                {
                                    //Find the piece on row
                                    for (int j = 0; j < table.GetLength(1); j++)
                                    {
                                        if (table[multiControl, j, 0] == validPieces[i, 0] && table[multiControl, j, 1] == turn)
                                        {
                                            //Valid piece is attained to the first index of the validpieces
                                            for (int k = 0; k < 2; k++)
                                                validPieces[0, k + 1] = validPieces[i, k + 1];
                                            clarify++;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    //If more than one move still exists after clarification or no move is found with clarification valid turns back to false
                    //If there were more than one valid moves and either more than one found valid or none found
                    if (validPieces[1, 0] != 0 && (clarify > 1 || clarify == 0))
                        valid = false;

                    //Change the piece's place
                    if (valid)
                    {
                        if (shortCastleBlue || shortCastleRed || longCastleBlue || longCastleRed)
                            Castling(table);

                        //Check control false means move is not available due to creating or not lifting check on it's own king
                        if (!shortCastleBlue && !shortCastleRed && !longCastleBlue && !longCastleRed)
                            checkControl = CheckSitutation(table, piece, (3 * validPieces[0, 1] + 3), (2 * validPieces[0, 2] + 1), movePos[0], movePos[1]);
                        else
                            checkControl = true;

                        if (checkControl)
                        {
                            if (!shortCastleBlue && !shortCastleRed && !longCastleBlue && !longCastleRed)
                            {
                                //Control if check notation was correct by placing the piece and controlling check afterwards
                                //Empty the first position
                                //Note: Char values are used as indices without converting to int
                                table[validPieces[0, 2], validPieces[0, 1], 0] = '.';
                                table[validPieces[0, 2], validPieces[0, 1], 1] = '*';
                                //Place Piece
                                for (int k = 0; k < 2; k++)
                                {
                                    landedOnPiece[k] = table[(movePos[1] - 1) / 2, (movePos[0] - 3) / 3, k];
                                    table[(movePos[1] - 1) / 2, (movePos[0] - 3) / 3, k] = piece[k];
                                }

                                ThreatTable(table);
                                if ((checkOnBlue || checkOnRed) && checkNot == ' ')
                                    valid = false;
                            }

                            if (valid)
                            {
                                if (promotion != ' ')
                                {
                                    if (promotion == 'N')
                                        PawnPromote(piece, "knight");
                                    else if (promotion == 'Q')
                                        PawnPromote(piece, "queen");
                                    else if (promotion == 'R')
                                        PawnPromote(piece, "rook");
                                    else
                                        PawnPromote(piece, "bishop");
                                }
                                if (enPassantNotation)
                                {
                                    if (turn == '+')
                                    {
                                        table[3, (movePos[0] - 3) / 3, 0] = '.';
                                        table[3, (movePos[0] - 3) / 3, 1] = '*';
                                    }
                                    else
                                    {
                                        table[4, (movePos[0] - 3) / 3, 0] = '.';
                                        table[4, (movePos[0] - 3) / 3, 1] = '*';
                                    }
                                    enPassant[(movePos[0] - 3) / 3] = false;
                                    enPassantNotation = false;
                                }

                                moves[movesIndex] = move;
                                movesIndex++;

                                counters = MovesTable(move, turn, counters);

                                //Switch Turns and Write Move Table
                                if (turn == '+')
                                    turn = '-';
                                else
                                    turn = '+';

                                endgame = false;

                                if (checkOnBlue || checkOnRed)
                                {
                                    positionofThePieceXY[0] = (movePos[0] - 3) / 3;
                                    positionofThePieceXY[1] = (movePos[1] - 1) / 2;

                                    temptable = table;
                                    endgame = EndGame(temptable, turn, positionofThePieceXY);
                                    if (turn == '+')
                                    {
                                        if (!blueThreat[(movePos[1] - 1) / 2, (movePos[0] - 3) / 3] || endgame)
                                            endgame = true;
                                    }
                                    else
                                    {
                                        if (!redThreat[(movePos[1] - 1) / 2, (movePos[0] - 3) / 3] || endgame)
                                            endgame = true;
                                    }
                                }
                                draw = Draw(table, turn);
                                if (endgame == true || draw == true)
                                {
                                    RefreshBoard(table);
                                    Console.SetCursorPosition(0, 27);
                                    Console.Write("Game Over.Write \"save\" to save the game or anything else to exit: ");
                                    move = Console.ReadLine();
                                    //Save Moves
                                    if (move == "save")
                                    {
                                        SaveMoves(moves);
                                        Console.SetCursorPosition(0, 27);
                                        Console.WriteLine("Game is saved to the source code path in the name of Previous_Game.txt.");
                                    }
                                    break;
                                }
                            }
                            else
                            {
                                for (int k = 0; k < 2; k++)
                                {
                                    table[(movePos[1] - 1) / 2, (movePos[0] - 3) / 3, k] = landedOnPiece[k];
                                    table[validPieces[0,2],validPieces[0,1], k] = piece[k];
                                }
                                Console.SetCursorPosition(0, 27);
                                Console.WriteLine("Please use check notation.");
                            }
                            RefreshBoard(table);
                        }
                        else
                        {
                            Console.SetCursorPosition(0, 27);
                            Console.WriteLine("Invalid move.");
                        }
                    }
                    //Enters here if more than one piece can move to the position and is not clarified which one
                    else if (validPieces[1, 0] != 0)
                        Console.WriteLine("Two of the same kind of piece can capture or move please show in notation it's row or column");
                    //If move is not valid enters here
                    else if (!valid)
                    {
                        if (demoMode == true)
                        {
                            Console.SetCursorPosition(0, 27);
                            Console.WriteLine("There is an invalid move in the file please correct it before playing.");
                            break;
                        }
                        else
                            Console.WriteLine("Invalid move");
                    }
                }
                else
                {
                    if (demoMode == true)
                    {
                        Console.SetCursorPosition(0, 27);
                        Console.WriteLine("There is an invalid move in the file please correct it before playing.");
                        break;
                    }
                    else
                        Console.WriteLine("Invalid move");
                }
            }
        }
        static void Main()
        {
            Console.SetWindowSize(131, 30);
            char[,,] table = Start();
            string playMode, path;
            int inputAttempts = -1;
            Console.Write("\n\n\nPlease write:\n\"cursor\" for cursor play mode. Press \"space\" to save the game. Press \"H\" for hints" +
                                              "\n\"string\" for string play mode. Write \"save\" to save the game. Write \"hint\" for hints." +
                                              "\n\"demo\"   for demo   play mode." +
                                              " \nChoose a play mode: ");
            do
            {
                playMode = Console.ReadLine().ToLower();
                inputAttempts++;
            } while (playMode != "cursor" && playMode != "string" && playMode != "demo");
            
            //Clear user prompt and inputs from the screen
            for (int i = 0; i < 6 + inputAttempts; i++)
            {
                Console.SetCursorPosition(0, 18 + i);
                for (int j = 0; j < 140; j++)
                    Console.Write(" ");
            }
            string[] moves = new string[500];
            string[] demoMoves;

            if (playMode == "cursor")
                CursorPlayMode(table, 1, 1, 0, '+', 0, moves);

            else if (playMode == "string")
                StringPlayMode(table, false, null, moves);

            else if (playMode == "demo")
            {
                Console.SetCursorPosition(0, 27);
                Console.Write("Please write the path or the name of the file that contains the moves.\nPlease write the name correctly to avoid getting any errors: ");
                path = Console.ReadLine();
                demoMoves = ReadMoves(path);
                StringPlayMode(table, true, demoMoves, moves);
            }
        }
    }
}
