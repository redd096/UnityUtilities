using redd096.Attributes;
using UnityEngine;

#if PIXELCRUSHER_DIALOGUESYSTEM
using PixelCrushers.DialogueSystem;
#endif

namespace redd096.DialogueSystem
{
#if PIXELCRUSHER_DIALOGUESYSTEM
    /// <summary>
    /// Attached to DialogueManager prefab to receive events. This is used just to debug how it works
    /// </summary>
    public class TestDialogueManager : MonoBehaviour
    {
        [SerializeField] string dialogueName = "TestDialogue";
        [SerializeField] string variableName = "TestVariable";
        [SerializeField] string fieldName = "TestField";

        private Subtitle currentSubtitle;

        #region buttons

        [Button]
        void StartDialogue()
        {
            DialogueManager.StartConversation(dialogueName);
        }

        [Button]
        void StopDialogue()
        {
            DialogueManager.StopConversation();
        }

        [Button]
        void CheckDialogueExists()
        {
            Debug.Log(DialogueManager.masterDatabase.GetConversation(dialogueName) != null);
        }

        [Button]
        void IsDialoguePlaying()
        {
            Debug.Log(DialogueManager.conversationView != null);
        }

        [Button]
        void CheckVariableExists()
        {
            Lua.Result result = DialogueLua.GetVariable(variableName);
            Debug.Log(result.hasReturnValue);
        }

        [Button]
        void CheckSubtitleHasField()
        {
            if (currentSubtitle == null)
            {
                Debug.Log("There is not subtitle active now");
            }

            Field result = Field.Lookup(currentSubtitle.dialogueEntry.fields, fieldName);
            Debug.Log(result != null);
        }

        #endregion

        #region events

        public void OnConversationStart(Transform actor)
        {
            Debug.Log($"On Conversation Start {actor}");
        }

        public void OnConversationEnd(Transform actor)
        {
            Debug.Log($"On Conversation End {actor}");
        }

        public void OnConversationCancelled(Transform actor)
        {
            Debug.Log($"On Conversation Cancelled {actor}");
        }

        public void OnConversationLine(Subtitle subtitle)
        {
            Debug.Log($"On Conversation Line {subtitle}");
            currentSubtitle = subtitle;
        }

        public void OnConversationLineEnd(Subtitle subtitle)
        {
            Debug.Log($"On Conversation Line End {subtitle}");
            currentSubtitle = null;
        }

        public void OnConversationLineCancelled(Subtitle subtitle)
        {
            Debug.Log($"On Conversation Line Cancelled {subtitle}");
        }

        public void OnConversationResponseMenu(Response[] responses)
        {
            Debug.Log($"On Conversation Response Menu {responses.Length}");
        }

        public void OnConversationTimeout()
        {
            Debug.Log($"On Conversation Timeout");
        }

        public void OnLinkedConversationStart(Transform actor)
        {
            Debug.Log($"On Linked Conversation Start {actor}");
        }

        #endregion
    }
#endif
}