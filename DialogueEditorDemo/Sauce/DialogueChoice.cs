using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Mahrq
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "New Choice", menuName = "Dialogue/Dialogue Choice", order = 1)]
    public class DialogueChoice : ScriptableObject
    {
        [SerializeField]
        [TextArea(5,5)]
        private string _text;
        [SerializeField]
        private int _fontSize = 12;
        [SerializeField]
        private int _choiceResult;

        public void SetChoiceText(ref Text textChoice)
        {
            textChoice.text = _text;
        }
        public string Text => _text;
        public int FontSize => _fontSize;
        public int ChoiceResult { get { return _choiceResult; } set { _choiceResult = value; } }
    }
}


