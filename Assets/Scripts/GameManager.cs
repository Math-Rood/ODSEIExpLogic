using UnityEngine;
using UnityEngine.UI; 
using System.Collections.Generic; 
using System.Collections;
using TMPro; 

public class GameManager : MonoBehaviour
{
    
    public GridManager gridManager;
    public PlayerController playerController;
    public CommandDropZone commandDropZone;
    public TextMeshProUGUI tutorialText; 
    public Button executeButton; 
    public Button resetButton;   
    public Transform commandPalette; 

    
    public GameObject commandMoveForwardPrefab;
    public GameObject commandTurnRightPrefab;
    public GameObject commandTurnLeftPrefab;

    public List<LevelData> levels; 
    private int currentLevelIndex = 0; 

        private int playerCurrentScore = 0; 
    private bool isExecutingCommands = false; 

    
    void Start()
    {
        InitializeGame(); 
    }

    
    void InitializeGame()
    {
        
        if (levels == null || levels.Count == 0)
        {
            Debug.LogError("Nenhum nível atribuído ao GameManager! Por favor, configure a lista 'levels' no Inspector.");
            NotifyPlayer("Erro: Níveis não configurados. Verifique o GameManager.");
            return;
        }

        
        if (currentLevelIndex >= levels.Count)
        {
            NotifyPlayer("Você completou todas as fases! Parabéns!");
            executeButton.interactable = false; 
            resetButton.interactable = false;
            return;
        }

        LevelData currentLevel = levels[currentLevelIndex]; 

        playerCurrentScore = 0; 
        isExecutingCommands = false; 

        
        executeButton.interactable = true;
        resetButton.interactable = true;

        
        ResetCommandsUI(currentLevel.availableCommands);

        
        gridManager.InitializeGrid();
        
        gridManager.currentGrid = currentLevel.GetGridData(gridManager.gridSize);
        
        for (int y = 0; y < gridManager.gridSize; y++)
        {
            for (int x = 0; x < gridManager.gridSize; x++)
            {
                gridManager.UpdateTileVisual(x, y, gridManager.currentGrid[x, y]);
            }
        }

        
        Vector2Int startPos = FindTilePosition(TileType.Start);
        if (startPos.x != -1) 
        {
            playerController.InitializePlayer(startPos.x, startPos.y, currentLevel.startDirection);
        }
        else
        {
            Debug.LogError($"Nível {currentLevel.levelID} não tem um tile de início (TileType.Start)! Jogador inicializado em (0,0).");
            playerController.InitializePlayer(0, 0, currentLevel.startDirection); 
        }

        
        NotifyPlayer(currentLevel.tutorialMessage);

        
        executeButton.onClick.RemoveAllListeners(); 
        executeButton.onClick.AddListener(ExecuteCommands);

        resetButton.onClick.RemoveAllListeners();
        resetButton.onClick.AddListener(ResetLevel);
    }

    
    private Vector2Int FindTilePosition(TileType typeToFind)
    {
        for (int y = 0; y < gridManager.gridSize; y++)
        {
            for (int x = 0; x < gridManager.gridSize; x++)
            {
                if (gridManager.currentGrid[x, y] == typeToFind)
                {
                    return new Vector2Int(x, y);
                }
            }
        }
        return new Vector2Int(-1, -1); 
    }


    public void ResetCommandsUI(CommandType[] availableCommands)
    {
        commandDropZone.ClearCommands(); 

        
        foreach (Transform child in commandPalette)
        {
            Destroy(child.gameObject);
        }

        
        foreach (CommandType type in availableCommands)
        {
            GameObject commandPrefabToInstantiate = null;
            
            switch (type)
            {
                case CommandType.MoveForward: commandPrefabToInstantiate = commandMoveForwardPrefab; break;
                case CommandType.TurnRight: commandPrefabToInstantiate = commandTurnRightPrefab; break;
                case CommandType.TurnLeft: commandPrefabToInstantiate = commandTurnLeftPrefab; break;
                default:
                    Debug.LogWarning($"Prefab de comando para tipo {type} não encontrado.");
                    continue; 
            }

            if (commandPrefabToInstantiate != null)
            {
                
                GameObject cmd = Instantiate(commandPrefabToInstantiate, commandPalette);
                
                cmd.GetComponent<DraggableCommand>().commandType = type;
            }
        }
    }

    
    void ResetLevel()
    {
        InitializeGame(); 
        NotifyPlayer("Nível reiniciado. Tente novamente!");
    }

    
    void ExecuteCommands()
    {
        
        if (isExecutingCommands || commandDropZone.currentCommands.Count == 0)
        {
            NotifyPlayer("Nenhum comando para executar ou comandos já em execução.");
            return;
        }

        isExecutingCommands = true; 
        executeButton.interactable = false; 
        resetButton.interactable = false;   

        
        StartCoroutine(ExecuteCommandsCoroutine(commandDropZone.currentCommands));
    }

    
    IEnumerator ExecuteCommandsCoroutine(List<DraggableCommand> commandsToExecute)
    {
        
        for (int i = 0; i < commandsToExecute.Count; i++)
        {
            if (!isExecutingCommands) { yield break; }

            DraggableCommand cmd = commandsToExecute[i];
            NotifyPlayer($"Executando: {cmd.commandType}..."); 

            IEnumerator actionCoroutine = null; 

            
            switch (cmd.commandType)
            {
                case CommandType.MoveForward: actionCoroutine = playerController.MoveForward(); break;
                case CommandType.TurnRight: actionCoroutine = playerController.TurnRight(); break;
                case CommandType.TurnLeft: actionCoroutine = playerController.TurnLeft(); break;
            }

            
            if (actionCoroutine != null)
            {
                yield return StartCoroutine(actionCoroutine);
            }
            else
            {
                
                yield return new WaitForSeconds(0.1f);
            }

        }

        
        NotifyPlayer("Todos os comandos executados. Verificando resultado...");
        yield return new WaitForSeconds(0.5f); 

        
        gridManager.CheckPlayerFinalPosition(playerController.currentX, playerController.currentY);

        isExecutingCommands = false; 
        executeButton.interactable = true; 
        resetButton.interactable = true;   
    }

    
    public void NotifyPlayer(string message)
    {
        if (tutorialText != null)
        {
            tutorialText.text = message;
        }
    }

    public void AddScore(int amount)
    {
        playerCurrentScore += amount;
        NotifyPlayer($"Tesouro coletado! Pontuação: {playerCurrentScore}");
        Debug.Log($"Pontuação atual: {playerCurrentScore}");
    }

    
    public void GameOver()
    {
        NotifyPlayer($"Fim de Jogo! Pontuação Final: {playerCurrentScore}. Clique em RESETAR para tentar novamente.");
        isExecutingCommands = false; 
        executeButton.interactable = false; 
        resetButton.interactable = true; 
    }

    public void LevelComplete()
    {
        NotifyPlayer($"Fase Completa! Pontuação: {playerCurrentScore}. Parabéns!");
        isExecutingCommands = false; 
        executeButton.interactable = false; 
        resetButton.interactable = true; 

        currentLevelIndex++; 

        if (currentLevelIndex < levels.Count)
        {
            
            StartCoroutine(LoadNextLevelAfterDelay(2f));
        }
        else
        {
            
            NotifyPlayer("Você completou todas as fases! Fim do Jogo. Obrigado por jogar!");
        }
    }

    
    IEnumerator LoadNextLevelAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        InitializeGame(); 
    }
}