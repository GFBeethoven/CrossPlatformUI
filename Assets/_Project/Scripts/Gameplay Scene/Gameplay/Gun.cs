using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Gun
{
    private const float BaseSensivity = 10.0f;
    private static readonly Vector3 GunOffset = new Vector3(3.0f, -3.0f);

    public event Action<Vector2> OnShot;

    private Transform _cross;
    private Transform _gun;

    public IGameplayInputProvider GameplayInputProvider { get; set; }

    private GameSettings _gameSettings;

    private bool _isAlreadyShot;

    public Gun(Transform cross, Transform gun,
        GameSettings gameSettings)
    {
        _cross = cross;
        _gun = gun;
        _gameSettings = gameSettings;

        _isAlreadyShot = false;
    }

    public void Tick()
    {
        if (GameplayInputProvider == null) return;

        Vector2 movement = GameplayInputProvider.ControlAxis * (Time.deltaTime * BaseSensivity);
        movement.x *= _gameSettings.HorizontalSensivity.Value;
        movement.y *= _gameSettings.VerticalSensivity.Value * (_gameSettings.InverseYAxis.Value ? -1.0f : 1.0f);

        _cross.transform.position += (Vector3)movement;
        _gun.transform.position = _cross.transform.position + GunOffset;

        if (!_isAlreadyShot)
        {
            if (GameplayInputProvider.IsShootingDown)
            {
                _isAlreadyShot = true;

                OnShot?.Invoke(_cross.transform.position);
            }
        }
        
        if (GameplayInputProvider.IsShootingUp)
        {
            _isAlreadyShot = false;
        }
    }
}
