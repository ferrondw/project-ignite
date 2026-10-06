using UnityEngine;
using UnityEngine.SceneManagement;

public class ThatGuyFinder : MonoBehaviour
{
    public Transform _cam;

    private void Update()
    {
        if (Physics.Raycast(_cam.position, _cam.eulerAngles, out var hit))
        {
            if (hit.transform.CompareTag("Guy"))
            {
                SceneManager.LoadScene("Win");
            }
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            SceneManager.LoadScene("Win");
        }
    }
}
