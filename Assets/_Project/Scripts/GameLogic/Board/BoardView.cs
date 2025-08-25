using R3;
using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Assets._Project.Scripts
{
    public interface IBoardView
    { 
        Vector3[] GetCellPositions();
        int GetCellIndexFromPosition(Vector3 position);
    }

    public class BoardView : MonoBehaviour, IBoardView
    {
        [SerializeField] private Button[] _cellButtons;
        private IBoardViewModel _boardViewModel;
        private CompositeDisposable _disposables = new();
        [Inject]
        public void Construct(NetworkMediator networkMediator, IBoardViewModel boardViewModel)
        {
            _boardViewModel = boardViewModel;
            _boardViewModel.Initialize(this,networkMediator);
        }
        private void Start()
        {
            Init();
        }
        private void Init()
        {
            for (int i = 0; i < _cellButtons.Length; i++)
            {
                int index = i;
                _cellButtons[index].onClick.AddListener(() => OnCellClicked(index));
            }

            SetupBindings();
        }

        private void SetupBindings()
        {
            if (_boardViewModel.CurrentPlayer != null)
            {
                _boardViewModel.CurrentPlayer.Subscribe(state =>
                {
                }).AddTo(_disposables);
            }

            if (_boardViewModel.GameResult != null)
            {
                _boardViewModel.GameResult.Subscribe(result =>
                {
                }).AddTo(_disposables);
            }
        }

        public Vector3[] GetCellPositions()
        {
            Vector3[] positions = new Vector3[_cellButtons.Length];
            for (int i = 0; i < _cellButtons.Length; i++)
            {
                positions[i] = _cellButtons[i] != null ? _cellButtons[i].image.rectTransform.anchoredPosition : Vector3.zero;
            }
            return positions;
        }

        public int GetCellIndexFromPosition(Vector3 position)
        {
            for (int i = 0; i < _cellButtons.Length; i++)
            {
                if (_cellButtons[i] != null && Vector3.Distance(_cellButtons[i].image.rectTransform.anchoredPosition, position) < 0.1f)
                {
                    return i;
                }
            }
            return -1;
        }

        private void OnCellClicked(int index)
        {
            RectTransform rectTransform = _cellButtons[index].GetComponent<RectTransform>();
            Vector2 anchoredPos = rectTransform.anchoredPosition;
 
            _boardViewModel.ClickCell(anchoredPos);
        }


        private void OnDestroy()
        {
            _boardViewModel?.Dispose();
            _disposables?.Dispose();
        }
    }
}