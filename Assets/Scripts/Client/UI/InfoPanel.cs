using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Client.UI {
    public class InfoPanel : MonoBehaviour {
        [SerializeField] 
        private Text _Text;

        private Coroutine _ShowCoroutine;
        
        private void OnValidate() {
            _Text = GetComponentInChildren<Text>();
        }

        public void ShowText(string text, float timeToShow) {
            _Text.text = text;
            if (_ShowCoroutine != null)
                StopCoroutine(_ShowCoroutine);
            _ShowCoroutine = StartCoroutine(ShowTextCoroutine(timeToShow));
        }

        private IEnumerator ShowTextCoroutine(float timeToShow) {
            _Text.gameObject.SetActive(true);
            yield return new WaitForSeconds(timeToShow);
            _Text.gameObject.SetActive(false);
            _ShowCoroutine = null;
        }
    }
}