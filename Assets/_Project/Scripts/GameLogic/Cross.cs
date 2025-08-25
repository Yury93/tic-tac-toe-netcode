using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Scripts.GameLogic
{ 
    public interface IColorable
    {
        void SetColor(Color color);
    }
    public class Cross : NetworkBehaviour, IColorable
    {
        [SerializeField] private Image _image;  
        public void SetColor(Color color)
        {
          
            if (_image != null)
            {
                _image.color = color;
            }

        }
    }
}