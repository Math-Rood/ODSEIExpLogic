using UnityEngine;

// Use [CreateAssetMenu] para poder criar esses objetos no Editor do Unity
[CreateAssetMenu(fileName = "NewLevel", menuName = "Game/Level Data")]
public class LevelData : ScriptableObject
{
    public int levelID;
    public string levelName;
    [TextArea(3, 5)] // Para facilitar a edição de texto longo no Inspector
    public string tutorialMessage;

    public PlayerController.Direction startDirection; // Direção inicial do jogador

    // Representação do tabuleiro para este nível.
    // Array de strings para facilitar a visualização no Inspector.
    // Ex: "S.T.E" onde S=Start, .=Empty, T=Treasure, E=End
    //     "###." onde #=Wall, .=Empty
    public string[] gridLayout;

    // Lista de comandos disponíveis para este nível na paleta.
    // Ex: { CommandType.MoveForward, CommandType.MoveForward, CommandType.TurnRight }
    public CommandType[] availableCommands;

    // Métodos para converter o gridLayout de string[] para TileType[,]
    public TileType[,] GetGridData(int gridSize)
    {
        TileType[,] grid = new TileType[gridSize, gridSize];

        for (int y = 0; y < gridSize; y++)
        {
            if (y >= gridLayout.Length) continue; // Previne erro se o layout for menor que o grid
            string row = gridLayout[y];
            for (int x = 0; x < gridSize; x++)
            {
                if (x >= row.Length) continue; // Previne erro se a linha for menor que o grid

                switch (row[x])
                {
                    case 'S': grid[x, y] = TileType.Start; break;
                    case 'E': grid[x, y] = TileType.End; break;
                    case 'T': grid[x, y] = TileType.Treasure; break;
                    case '#': grid[x, y] = TileType.Wall; break; // Parede
                    case 'X': grid[x, y] = TileType.Trap; break;  // Armadilha
                    case 'M': grid[x, y] = TileType.Enemy; break; // Inimigo
                    case '.': grid[x, y] = TileType.Empty; break;
                    default: grid[x, y] = TileType.Empty; break; // Default para qualquer outro char
                }
            }
        }
        return grid;
    }
}