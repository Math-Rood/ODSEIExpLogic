using UnityEngine;
using System.Collections.Generic; 

public enum TileType { Empty, Start, End, Treasure, Wall, Enemy, Trap } // Adicionando mais tipos

public class GridManager : MonoBehaviour
{
   
    public GameObject tilePrefab; // Prefab com SpriteRenderer para o fundo das células
    public Sprite defaultTileSprite; // Sprite para células vazias
    public Sprite startTileSprite;   // Sprite para o ponto de partida
    public Sprite endTileSprite;     // Sprite para o ponto de chegada
    public Sprite treasureTileSprite; // Sprite para tesouros

    public int gridSize = 4;
    public float cellSize = 1.0f; // Tamanho de cada célula em unidades Unity

    
    public TileType[,] currentGrid; // Armazena os tipos de célula do tabuleiro atual
    private GameObject[,] tileObjects; // Guarda referências aos GameObjects visuais das células

    private GameManager gameManager; 

    void Awake() 
    {
        gameManager = FindObjectOfType<GameManager>(); 
        if (gameManager == null) Debug.LogError("GameManager not found!");
    }

    // Chamado pelo GameManager para configurar o tabuleiro inicial
    public void InitializeGrid()
    {
        // Limpa tabuleiro anterior se existir
        if (tileObjects != null)
        {
            foreach (GameObject obj in tileObjects)
            {
                if (obj != null) Destroy(obj);
            }
        }
        GenerateGridVisual(); // Cria os GameObjects das células
        LoadTutorial1Grid();  // Carrega o layout específico do Tutorial 1
    }

    // Cria os GameObjects visuais das células do tabuleiro
    void GenerateGridVisual()
    {
        tileObjects = new GameObject[gridSize, gridSize];
        for (int y = 0; y < gridSize; y++)
        {
            for (int x = 0; x < gridSize; x++)
            {
                Vector3 position = new Vector3(x * cellSize, y * cellSize, 0); // Posição 2D
                GameObject tileInstance = Instantiate(tilePrefab, position, Quaternion.identity, transform);
                tileInstance.name = $"Tile_{x}_{y}";
                tileObjects[x, y] = tileInstance; // Armazena a referência

                SpriteRenderer sr = tileInstance.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.sortingOrder = 0; // Camada base para as células
                }
            }
        }
    }

    // Define o layout do tabuleiro para a Fase Tutorial 1 (Andar para Frente)
    public void LoadTutorial1Grid()
    {
        currentGrid = new TileType[gridSize, gridSize];

        // Preenche todas as células como vazias por padrão
        for (int y = 0; y < gridSize; y++)
        {
            for (int x = 0; x < gridSize; x++)
            {
                currentGrid[x, y] = TileType.Empty;
                UpdateTileVisual(x, y, TileType.Empty); // Garante que o visual seja o padrão
            }
        }

        // Define o caminho para o tutorial 1 (Exemplo: caminho reto na primeira linha)
        // S _ T E
        // _ _ _ _
        // _ _ _ _
        // _ _ _ _
        currentGrid[0, 0] = TileType.Start;
        currentGrid[1, 0] = TileType.Empty; // Célula vazia no caminho
        currentGrid[2, 0] = TileType.Treasure; // Tesouro no caminho
        currentGrid[3, 0] = TileType.End; // Ponto final

        // Atualiza os visuais das células específicas
        UpdateTileVisual(0, 0, TileType.Start);
        UpdateTileVisual(2, 0, TileType.Treasure);
        UpdateTileVisual(3, 0, TileType.End);

        Debug.Log("Tutorial 1 Grid Loaded.");
    }

    // Atualiza o sprite de uma célula específica
    public void UpdateTileVisual(int x, int y, TileType type)
    {
        if (x < 0 || x >= gridSize || y < 0 || y >= gridSize || tileObjects[x, y] == null) return;

        SpriteRenderer sr = tileObjects[x, y].GetComponent<SpriteRenderer>();
        if (sr == null) return;

        switch (type)
        {
            case TileType.Empty: sr.sprite = defaultTileSprite; break;
            case TileType.Start: sr.sprite = startTileSprite; break;
            case TileType.End: sr.sprite = endTileSprite; break;
            case TileType.Treasure: sr.sprite = treasureTileSprite; break;
            // TODO: Adicionar sprites para Enemy e Trap quando implementá-los
            default: sr.sprite = defaultTileSprite; break;
        }
        sr.sortingOrder = 0; // Garante que os tiles estejam na camada de fundo
    }

    // Verifica o que há na posição do jogador após cada movimento
    public void CheckPlayerPosition(int x, int y)
    {
        if (x < 0 || x >= gridSize || y < 0 || y >= gridSize)
        {
            gameManager.NotifyPlayer("Você saiu do tabuleiro! Game Over.");
            gameManager.GameOver();
            return;
        }

        TileType currentTile = currentGrid[x, y];
        switch (currentTile)
        {
            case TileType.Treasure:
                gameManager.AddScore(1);
                gameManager.NotifyPlayer("Tesouro coletado!");
                currentGrid[x, y] = TileType.Empty; // Remove o tesouro do grid de dados
                UpdateTileVisual(x, y, TileType.Empty); // Remove o visual do tesouro
                break;
            case TileType.Enemy:
                gameManager.NotifyPlayer("Você colidiu com um inimigo! Game Over.");
                gameManager.GameOver();
                break;
            case TileType.Trap:
                gameManager.NotifyPlayer("Você caiu em uma armadilha! Game Over.");
                gameManager.GameOver();
                break;
            case TileType.End:
                // Não faz nada aqui, a checagem final será feita no GameManager
                break;
            case TileType.Empty:
            case TileType.Start:
                // Nada especial acontece nessas células
                break;
        }
    }

    // Verifica a condição final do jogador após a execução de todos os comandos
    public void CheckPlayerFinalPosition(int x, int y)
    {
        if (x < 0 || x >= gridSize || y < 0 || y >= gridSize)
        {
            gameManager.NotifyPlayer("Você terminou fora do tabuleiro! Tente novamente.");
            gameManager.GameOver(); // Trata como game over se terminou fora
            return;
        }

        TileType finalTile = currentGrid[x, y];
        if (finalTile == TileType.End)
        {
            gameManager.NotifyPlayer("Parabéns! Você chegou ao final da fase!");
            gameManager.LevelComplete();
        }
        else
        {
            gameManager.NotifyPlayer("Sua sequência de comandos não o levou ao final. Tente novamente!");
            gameManager.GameOver(); // Ou apenas um "Tente novamente" sem resetar se preferir
        }
    }
}