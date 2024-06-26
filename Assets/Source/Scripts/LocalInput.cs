using System.Collections.Generic;
using Source.Scripts.Multiplayer;
using Source.Scripts.StaticData;
using UnityEngine;

namespace Source.Scripts
{
    public class LocalInput : MonoBehaviour
    {
        [SerializeField] private Transform _cursor;
        [SerializeField]private PlayerAim _playerAimPrefab;

        private MultiplayerManager _multiplayerManager;
        private Camera _camera;
        private Plane _plane;
        private PlayerAim _playerAim;

        public void Init(Transform snake, PlayerStaticData playerSettings)
        {
            _multiplayerManager = MultiplayerManager.Instance;
            _camera = Camera.main;
            _plane = new Plane(Vector3.up,Vector3.zero);
            _playerAim = Instantiate(_playerAimPrefab, snake.position, snake.rotation);
            _playerAim.Init(playerSettings.Speed, playerSettings.RotateSpeed);
        }

        public void Destroy()
        {
            _playerAim.Destroy();
            Destroy(gameObject);
        }

        private void Update()
        {
            if (Input.GetMouseButton(0))
            {
                MoveCursor();
                _playerAim.SetTarget(_cursor.position);
            }
            
            SendMove();
        }

        private void SendMove()
        {
            _playerAim.GetMoveInfo(out Vector3 position);

            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                { "x", position.x },
                { "z", position.z },
            };
            
            _multiplayerManager.SendMessage(MessageNames.move, data);
        }

        private void MoveCursor()
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (_plane.Raycast(ray, out float distance)) 
                _cursor.position = ray.GetPoint(distance);
        }
    }
}
