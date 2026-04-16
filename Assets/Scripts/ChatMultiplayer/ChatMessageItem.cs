using TMPro;
using UnityEngine;

public class ChatMessageItem : MonoBehaviour
{
    [SerializeField] private TMP_Text username;

    [SerializeField] private TMP_Text text;

    public void Setup(string usernameValue, string message, bool isMine)
    {
        username.color = GetColorForUser(usernameValue);
        username.text = usernameValue;
        text.text = message;

        if (isMine)
        {
            text.alignment = TextAlignmentOptions.Left;
            username.alignment = TextAlignmentOptions.Left;
        }
        else
        {
            text.alignment = TextAlignmentOptions.Right;
            username.alignment = TextAlignmentOptions.Right;
        }
    }

    private Color GetColorForUser(string username)
    {
        int hash = username.GetHashCode();
        Random.InitState(hash);

        return new Color(
            Random.value,
            Random.value,
            Random.value
        );
    }
}