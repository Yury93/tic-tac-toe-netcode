using Assets._Project.Scripts.Assets._Project.Scripts;
using R3;
using System;
using UnityEngine;
using Zenject;

namespace Assets._Project.Scripts
{
    public interface IBoardViewModel : IDisposable
    {
        void Initialize(IBoardView view, NetworkMediator networkMediator);
        void ClickCell(Vector3 position);
        CellModel GetCell(int row, int col);
        ReadOnlyReactiveProperty<StateCell> CurrentPlayer { get; }
        ReadOnlyReactiveProperty<string> GameResult { get; }
    }

    public class BoardViewModel : IBoardViewModel
    {
        private IBoardModel _model;
        private IBoardView _view;
        private NetworkMediator _networkMediator;

        private Subject<Vector3> _cellClicked = new();
        private CompositeDisposable _disposables = new CompositeDisposable();


        private ReactiveProperty<StateCell> _currentPlayer = new(StateCell.Player1);
        private ReactiveProperty<string> _gameResult = new("Ходит игрок 1");

        public ReadOnlyReactiveProperty<StateCell> CurrentPlayer => _currentPlayer;
        public ReadOnlyReactiveProperty<string> GameResult => _gameResult;

        [Inject]
        public BoardViewModel(IBoardModel model)
        {
            _model = model;
        }
        public void Initialize(IBoardView view, NetworkMediator networkMediator)
        {
            _view = view;
            _networkMediator = networkMediator;

            InitializeCells();
            SetupSubscriptions();
            SetupCellClickHandler();
        }

        private void InitializeCells()
        {
            Vector3[] positions = _view.GetCellPositions();
            int index = 0;

            for (int i = 0; i < Constants.BOARD_SIZE; i++)
            {
                for (int j = 0; j < Constants.BOARD_SIZE; j++)
                {
                    if (index < positions.Length)
                    {
                        _model.CreateCell(i, j, positions[index]);
                        index++;
                    }
                }
            }
        }

        private void SetupSubscriptions()
        {
            if (_networkMediator == null) return;

            _networkMediator.OnCellStateChanged.Subscribe(tuple =>
            {
                int index = tuple.index;
                StateCell state = tuple.state;
                int row = index / Constants.BOARD_SIZE;
                int col = index % Constants.BOARD_SIZE;

                _model.SetCellState(row, col, state);
            }).AddTo(_disposables);

            _networkMediator.OnCurrentPlayerChanged.Subscribe(player =>
            {
                _currentPlayer.Value = player;
                _gameResult.Value = player == StateCell.Player1 ? "Ходит игрок Х" : "Ходит игрок О";
            }).AddTo(_disposables);

            _networkMediator.OnGameEnded.Subscribe(result =>
            {
                switch (result)
                {
                    case MatchResult.Player1Wins:
                        _gameResult.Value = "Победил игрок Х!";
                        break;
                    case MatchResult.Player2Wins:
                        _gameResult.Value = "Победил игрок О!";
                        break;
                    case MatchResult.Draw:
                        _gameResult.Value = "Ничья!";
                        break;
                    default:
                        _gameResult.Value = "Игра окончена";
                        break;
                }
                Debug.Log(_gameResult.Value);
            }).AddTo(_disposables);
        }

        private void SetupCellClickHandler()
        {
            _cellClicked.Subscribe(position =>
            {
                int cellIndex = _view.GetCellIndexFromPosition(position);
                if (cellIndex >= 0)
                {
                    int row = cellIndex / Constants.BOARD_SIZE;
                    int col = cellIndex % Constants.BOARD_SIZE;

                    var cell = _model.GetCell(row, col);
                    if (cell != null && cell.stateCell == StateCell.Empty)
                    {
                        StateCell currentPlayer = _networkMediator.GetCurrentPlayer();
                        if (!_networkMediator.IsGameEnded() && _networkMediator.IsCellEmpty(cellIndex))
                        {
                            _networkMediator.MakeMove(cellIndex, currentPlayer,position);
                        }
                    }
                }
            }).AddTo(_disposables);
        }

        public void ClickCell(Vector3 position)
        {
            _cellClicked.OnNext(position);
        }

        public CellModel GetCell(int row, int col)
        {
            return _model.GetCell(row, col);
        }

        public void Dispose()
        {
            _disposables.Dispose();
            _cellClicked.Dispose();
            _currentPlayer.Dispose();
            _gameResult.Dispose();
        }
    }
}