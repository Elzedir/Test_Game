using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Manager_Menu : MonoBehaviour
{
    public static Manager_Menu Instance;

    public GameObject UICanvas;
    public Inventory_Window InventoryMenu;
    public Journal_Window JournalMenu;
    public Menu_Escape EscapeMenu;
    public Menu_CharacterSelect CharacterSelectMenu;
    private List<Transform> _menus = new();

    private void Start()
    {
        foreach (Transform child in UICanvas.transform)
        {
            _menus.Add(child);
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    public void HandleEscapePressed(Menu_UI menu = null, GameObject interactedObject = null)
    {
        if (menu != null)
        {
            menu.CloseMenu(interactedObject);
            SetWindowToBack(menu.gameObject);
        }

        else
        {
            List<GameObject> openUIWindows = new();

            foreach (Transform child in UICanvas.transform)
            {
                if (child.gameObject.activeSelf)
                {
                    openUIWindows.Add(child.gameObject);
                }
            }

            if (openUIWindows.Count > 0)
            {
                GameObject lastOpenWindow = openUIWindows.LastOrDefault();
                SetWindowToBack(lastOpenWindow);

                if (!OtherWindowOpen() && lastOpenWindow.TryGetComponent<Menu_UI>(out Menu_UI lastOpenMenuMenu))
                {
                    HandleEscapePressed(lastOpenMenuMenu, lastOpenMenuMenu.InteractedObject);
                }
            }
            else
            {
                Menu_Escape.Instance.OpenMenu();
            }
        }
    }

    public bool OtherWindowOpen()
    {
        if (Menu_Settings_Rebind.Instance.IsOpen)
        {
            return true;
        }

        return false;
    }

    public void OpenMenu(Menu_UI menu, GameObject interactedObject = null)
    {
        if (!menu.IsOpen)
        {
            menu.OpenMenu(interactedObject);
            SetWindowToFront(menu.gameObject);
        }
        else
        {
            int childCount = menu.transform.parent.childCount;

            if (menu.transform.GetSiblingIndex() == childCount - 1)
            {
                Manager_Menu.Instance.HandleEscapePressed(menu, interactedObject);
            }
            else
            {
                SetWindowToFront(menu.gameObject);
            }
        }
    }

    public void SetWindowToFront(GameObject window)
    {
        window.transform.SetAsLastSibling();
    }
    public void SetWindowToBack(GameObject window)
    {
        window.transform.SetAsFirstSibling();
    }

    public T OpenWindowCheck<T>() where T : MonoBehaviour
    {
        foreach (Transform child in UICanvas.transform)
        {
            if (child.gameObject.activeSelf)
            {
                T window = child.GetComponent<T>();
                if (window != null)
                {
                    return window;
                }
            }
        }
        return null;
    }
}
