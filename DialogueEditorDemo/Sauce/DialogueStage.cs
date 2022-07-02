using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Mahrq
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "New DialogueStage", menuName = "Dialogue/Dialogue Stage", order = 0)]
    public class DialogueStage : ScriptableObject
    {
        [SerializeField]
        private string _speaker;
        [SerializeField]
        [TextArea(20, 20)]
        private string _text;
        [SerializeField]
        private int _fontSize = 12;
        [SerializeField]
        private DialogueChoice[] _choices;

        public void SetDialogueText(ref Text  textDialogue, ref Text textSpeaker)
        {
            textSpeaker.text = _speaker;
            textDialogue.text = _text;
            textDialogue.fontSize = _fontSize;
        }

        public string Speaker { get { return _speaker; } set { _speaker = value; } }
        public string Text { get { return _text; } set { _text = value; } }
        public int FontSize { get { return _fontSize; } set { _fontSize = value; } }
        public DialogueChoice[] Choices { get { return _choices; } set { _choices = value; } }

        public override bool Equals(object other)
        {
            if (other != null)
            {
                if (other is DialogueStage)
                {
                    return ((DialogueStage)other).Speaker == this.Speaker &&
                        ((DialogueStage)other).Text == this.Text &&
                        ((DialogueStage)other).FontSize == this.FontSize &&
                        ((DialogueStage)other).Choices == this.Choices;
                }
            }
            return false;         
        }
        public override int GetHashCode()
        {
            return (_speaker, _text, _fontSize, _choices).GetHashCode();
        }
    }
}

