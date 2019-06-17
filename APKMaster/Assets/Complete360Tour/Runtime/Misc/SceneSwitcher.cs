using UnityEngine;
using UnityEngine.SceneManagement;

namespace DigitalSalmon.C360
{
    public class SceneSwitcher : MonoBehaviour {
        //-----------------------------------------------------------------------------------------
        // Inspector Variables:
        //-----------------------------------------------------------------------------------------

        [Header("Required")] [SerializeField] protected string sceneName = "ExampleTour"; // Scene must exist in Build Settings.

        [Header("Optional")] [SerializeField] protected ScreenTint screenTint;

        //-----------------------------------------------------------------------------------------
        // Unity Lifecycle:
        //-----------------------------------------------------------------------------------------

        protected void Update() {
            if (Input.GetKeyDown(KeyCode.Space)) {
                LoadScene();
            }
        }

		
		//-----------------------------------------------------------------------------------------
		// Private Methods:
		//-----------------------------------------------------------------------------------------

		private void LoadScene() {
            if (screenTint == null) {
                SceneManager.LoadScene(sceneName);
                return;
            }

            // Note that the ScreenTint class does not handle overlapping fades. 
            // Using it out of the box does not guarentee the onComplete will call if something else tries to run a screen fade.

            screenTint.Fade(1, 1, () => SceneManager.LoadScene(sceneName));
        }
    }
}