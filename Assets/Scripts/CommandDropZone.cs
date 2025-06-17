using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;

public class CommandDropZone : MonoBehaviour, IDropHandler
{
    public Transform commandContainer;
    public List<DraggableCommand> currentCommands = new List<DraggableCommand>();

    void Awake()
    {
        
        if (commandContainer == null)
        {
            commandContainer = this.transform; 
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        DraggableCommand droppedCommand = eventData.pointerDrag.GetComponent<DraggableCommand>();
        if (droppedCommand != null)
        {
            
            droppedCommand.transform.SetParent(commandContainer);


            droppedCommand.rectTransform.localScale = Vector3.one; 

            currentCommands.Add(droppedCommand);
            Debug.Log($"Comando {droppedCommand.commandType} adicionado ao bloco de execução.");

            LayoutRebuilder.ForceRebuildLayoutImmediate(commandContainer.GetComponent<RectTransform>());
        }
    }

    public void ClearCommands()
    {
        foreach (DraggableCommand cmd in currentCommands)
        {
            if (cmd != null)
            {
                Destroy(cmd.gameObject);
            }
        }
        currentCommands.Clear();

        if (commandContainer != null && commandContainer.GetComponent<RectTransform>() != null)
        {
             LayoutRebuilder.ForceRebuildLayoutImmediate(commandContainer.GetComponent<RectTransform>());
        }
    }
}