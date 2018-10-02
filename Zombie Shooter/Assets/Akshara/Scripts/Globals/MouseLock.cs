using UnityEngine;

namespace AksharaMurda
{
    public class MouseLock : MonoBehaviour
    {
        private bool _isLocked = true;

        private void LateUpdate()
        {
            if (GlobalManager.instance.useMobileConsole)
                return;

            if (Input.GetKeyDown(KeyCode.Escape))
                _isLocked = false;

            if (Input.GetMouseButtonDown(0))
                _isLocked = true;

            if (_isLocked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }
}