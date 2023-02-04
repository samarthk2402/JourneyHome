using UnityEngine;

public class CursorScript : MonoBehaviour
{
    public bool isCursorLocked;

    void Start()
    {
        isCursorLocked = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || !isCursorLocked)
        {
            Cursor.lockState = CursorLockMode.None;
        }else{
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
