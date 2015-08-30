using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class ButtonTest : MonoBehaviour {
    private Button _button;
    private Color _color;
    public Image _testImage;
	// Use this for initialization
	void Start () {
        _button = GetComponent<Button>();
        _color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        GetComponent<Image>().color = _color;
        _button.onClick.AddListener(() =>
        {
            _testImage.color = _color;
        });
	}
	
}
