using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Pool;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;

public class LoadMenuView : HFSMStateView
{
    [SerializeField] private LoadSlotView _slotViewPrefab;

    [SerializeField] private Transform _layoutGroupContent;

    [SerializeField] private EventSystem _eventSystem;

    [SerializeField] private ControlGuide _controlGuide;

    [SerializeField] private Image _thumbnailFrame;

    [SerializeField] private RawImage _thumbnail;

    [SerializeField] private AspectRatioFitter _thumbnailFrameAspect;

    [SerializeField] private AspectRatioFitter _thumbnailAspect;

    [SerializeField] private ScrollRect _scrollRect;

    private LoadMenuViewModel _viewModel;

    private ObjectPool<LoadSlotView> _slotsPool;

    private Dictionary<LoadMenuViewModel.ViewData, LoadSlotView> _slotsMap = new();

    private InputDispatcher _inputDispatcher;

    private bool _isSetuped = false;

    private Vector2 _thumbnailFrameStartAnchoredPosition;

    private CoroutineHandle _showThumbnailCoroutine;

    private LoadSlotView _prevSelectedView;
    private GameObject _prevSelectedGameObject;

    public void Setup(LoadMenuViewModel viewModel, InputDispatcher inputDispatcher)
    {
        if (_isSetuped) return;

        _thumbnailFrameStartAnchoredPosition = _thumbnailFrame.rectTransform.anchoredPosition;

        _inputDispatcher = inputDispatcher;

        _slotsPool = new ObjectPool<LoadSlotView>
            (
            createFunc: () =>
            {
                var newObj = Instantiate(_slotViewPrefab, _layoutGroupContent);
                newObj.gameObject.SetActive(false);
                newObj.Initialize();
                return newObj;
            },
            actionOnGet: (s) =>
            {
                s.gameObject.SetActive(true);
                s.EventsHandler = this;
            },
            actionOnRelease: (s) =>
            {
                s.gameObject.SetActive(false);
                s.Thumbnail = null;
                s.EventsHandler = null;
            },
            actionOnDestroy: (s) =>
            {
                s.Thumbnail = null;
                s.EventsHandler = null;
                Destroy(s.gameObject);
            }
            );

        _viewModel = viewModel;

        _viewModel.AllViewData.OnItemAdd += OnViewDataAdd;
        _viewModel.AllViewData.OnItemRemove += OnViewDataRemove;
        _viewModel.AllViewData.OnBeforeClear += OnBeforeViewDataClear;

        _isSetuped = true;
    }

    public void Show()
    {
        _prevSelectedGameObject = null;
        _prevSelectedView = null;

        gameObject.SetActive(true);

        if (_viewModel.AllViewData.Count > 0)
        {
            if (_slotsMap.TryGetValue(_viewModel.AllViewData[0], out LoadSlotView slotView))
            {
                _eventSystem.SetSelectedGameObject(slotView.gameObject);
            }
        }

        _controlGuide.Show();
    }

    public void Hide()
    {
        gameObject.SetActive(false);

        _controlGuide.Hide();
    }

    public void HandleSlotClick(LoadSlotView slotView)
    {
        var slot = GetDataBySlot(slotView);

        _viewModel.LoadRequested(slot);
    }

    public void HandleSlotPointerEnter(LoadSlotView slotView)
    {
        if (_eventSystem.alreadySelecting) return;

        _prevSelectedView = slotView;
        _prevSelectedGameObject = slotView.gameObject;

        if (_eventSystem.currentSelectedGameObject != _prevSelectedGameObject)
        {
            _eventSystem.SetSelectedGameObject(_prevSelectedGameObject);
            ShowThumbnail(_prevSelectedView.Thumbnail);
        }
    }

    private void Update()
    {
        if (_inputDispatcher.UIInput.UIInputProvider.IsBackButtonDown)
        {
            AudioPlayer.Instance?.PlayNavigationBack();

            _viewModel.MainMenuRequested();
        }
        else if (_inputDispatcher.UIInput.UIInputProvider.IsDeleteSaveButtonDown)
        {
            var slot = GetDataBySlot(_eventSystem.currentSelectedGameObject?.GetComponent<LoadSlotView>());  

            _viewModel.DeleteRequested(slot);
        }
        //else if (_inputDispatcher.UIInput.UIInputProvider.IsConfirmButtonDown)
        //{
        //    var slot = GetDataBySlot(_eventSystem.currentSelectedGameObject?.GetComponent<LoadSlotView>());

        //    _viewModel.LoadRequested(slot);
        //}

        if (_eventSystem.currentSelectedGameObject != _prevSelectedGameObject)
        {
            _prevSelectedGameObject = _eventSystem.currentSelectedGameObject;

            if (_prevSelectedGameObject != null)
            {
                var slot = _prevSelectedGameObject.GetComponent<LoadSlotView>();
                if (slot != null)
                {
                    _prevSelectedView = slot;
                    if (_inputDispatcher.UIInputType.Value != InputSource.SourceType.KeyboardMouse)
                    {
                        _scrollRect.ScrollToCenter(_prevSelectedView.RectTransform);
                    }
                    else
                    {
                        _scrollRect.ScrollToShow(_prevSelectedView.RectTransform);
                    }
                    ShowThumbnail(_prevSelectedView.Thumbnail);
                }
                else
                {
                    _prevSelectedView = null;
                }
            }
        }
    }

    private LoadMenuViewModel.ViewData GetDataBySlot(LoadSlotView slot)
    {
        if (slot == null) return null;

        foreach (var pair in _slotsMap)
        {
            if (pair.Value == slot)
            {
                return pair.Key;
            }
        }

        return null;
    }

    private void OnViewDataAdd(int index, LoadMenuViewModel.ViewData viewData)
    {
        LoadSlotView newView = _slotsPool.Get();

        _slotsMap[viewData] = newView;

        newView.SetBestScore(viewData.BestScore);
        newView.SetCreationTime(viewData.CreationTime);
        newView.Thumbnail = viewData.Thumbnail;
    }

    private void OnViewDataRemove(int index, LoadMenuViewModel.ViewData viewData)
    {
        if (_slotsMap.TryGetValue(viewData, out var slot))
        {
            _slotsPool.Release(slot);
        }

        _slotsMap.Remove(viewData);
    }

    private void OnBeforeViewDataClear()
    {
        foreach (var slot in _slotsMap.Values)
        {
            _slotsPool.Release(slot);
        }

        _slotsMap.Clear();
    }

    private void ShowThumbnail(Texture2D thumbnail)
    {
        if (_showThumbnailCoroutine.IsValid)
        {
            Timing.KillCoroutines(_showThumbnailCoroutine);
        }

        if (thumbnail == null)
        {
            _thumbnailFrame.gameObject.SetActive(false);
        }
        else
        {
            _thumbnailFrame.gameObject.SetActive(true);

            _thumbnail.texture = thumbnail;
            _thumbnailAspect.aspectRatio = 1.0f * thumbnail.width / thumbnail.height;
            _thumbnailFrameAspect.aspectRatio = 1.0f * thumbnail.width / thumbnail.height;

            _showThumbnailCoroutine = Timing.RunCoroutine(_ShowThumbnail().CancelWith(gameObject));
        }
    }

    private IEnumerator<float> _ShowThumbnail()
    {
        const float Duration = 0.3f;
        const float OffsetY = -100.0f;

        Color trans = new Color(1.0f, 1.0f, 1.0f, 0.0f);

        Vector2 startPos = _thumbnailFrameStartAnchoredPosition;
        startPos.y += OffsetY;

        Vector2 endPos = _thumbnailFrameStartAnchoredPosition;

        for (float i = 0.0f; i < 1.0f; i += Timing.DeltaTime / Duration)
        {
            float t = Easing.OutQuad(i);

            _thumbnailFrame.rectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, t);

            Color color = Color.Lerp(trans, Color.white, t);

            _thumbnailFrame.color = color;
            _thumbnail.color = color;

            yield return Timing.WaitForOneFrame;
        }

        _thumbnailFrame.rectTransform.anchoredPosition = endPos;

        _thumbnailFrame.color = Color.white;
        _thumbnail.color = Color.white;

        _showThumbnailCoroutine = new CoroutineHandle();
    }
}
