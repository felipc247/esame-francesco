using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ButtonGroupSelector : MonoBehaviour
{
    [SerializeField] private List<Button> _buttons = new();

    private Button _currentButton;
    private Color _currentButtonNormalColor;

    private List<UnityAction> _buttonClickActions = new();

    void OnEnable()
    {
        int buttonsCount = _buttons.Count;
        for (int i = buttonsCount - 1; i >= 0; i--)
        {
            Button button = _buttons[i];
            if (button == null)
            {
                _buttons.RemoveAt(i);
                continue;
            }

            UnityAction onButtonClick = () => ButtonClick(button);
            _buttonClickActions.Add(onButtonClick);

            button.onClick.RemoveListener(onButtonClick);
            button.onClick.AddListener(onButtonClick);
        }
    }

    void OnDisable()
    {
        int buttonsCount = _buttons.Count;
        for (int i = buttonsCount - 1; i >= 0; i--)
        {
            Button button = _buttons[i];
            if (button == null)
            {
                _buttons.RemoveAt(i);
                continue;
            }

            UnityAction onClickButton = _buttonClickActions[i];

            if (onClickButton != null)
            {
                button.onClick.RemoveListener(onClickButton);
            }
        }
        _buttonClickActions.Clear();
    }

    public void ButtonClick(Button clickedButton)
    {
        if (clickedButton == null) return;

        if (_currentButton != null)
        {
            var colorBlockPreviousButton = _currentButton.colors;
            colorBlockPreviousButton.normalColor = _currentButtonNormalColor;
            _currentButton.colors = colorBlockPreviousButton;
        }

        _currentButton = clickedButton;
        _currentButtonNormalColor = clickedButton.colors.normalColor;
        var colorBlockNewButton = clickedButton.colors;
        colorBlockNewButton.normalColor = colorBlockNewButton.selectedColor;
        clickedButton.colors = colorBlockNewButton;
    }
}
