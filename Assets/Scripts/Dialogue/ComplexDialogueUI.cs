﻿using Assets.Scripts.Dialogue.Texts;
using System;
using System.Collections;
using UnityEngine;
using Yarn.Unity;

namespace Assets.Scripts.Dialogue
{
    public class ComplexDialogueUI : DialogueUI
    {
        private DialogueSnippetSystem[] snippetSystems;

        // When true, the user has indicated that they want to proceed to
        // the next line.
        private bool proceedToNextLine = false;

        void Start()
        {
            snippetSystems = FindObjectsOfType<DialogueSnippetSystem>();
        }

        public override Yarn.Dialogue.HandlerExecutionType RunLine(Yarn.Line line, ILineLocalisationProvider localisationProvider, Action onComplete)
        {
            // Start displaying the line; it will call onComplete later
            // which will tell the dialogue to continue
            StartCoroutine(DoRunLine(line, localisationProvider, onComplete));
            return Yarn.Dialogue.HandlerExecutionType.PauseExecution;
        }

        /// Show a line of dialogue, gradually        
        private IEnumerator DoRunLine(Yarn.Line line, ILineLocalisationProvider localisationProvider, Action onComplete)
        {
            onLineStart?.Invoke();

            proceedToNextLine = false;

            // The final text we'll be showing for this line.
            string text = localisationProvider.GetLocalisedTextForLine(line);

            if (text != null)
            {
                // Replace snippets with real text
                if (snippetSystems != null && snippetSystems.Length > 0)
                {
                    foreach (var snippetSystem in snippetSystems)
                    {
                        text = snippetSystem.ParseAndReplace(text, RunLineLogger);
                    }
                }
            }
            else
            {
                Debug.LogWarning($"Line {line.ID} doesn't have any localised text.");
                text = line.ID;
            }

            if (textSpeed > 0.0f)
            {
                IDialogueText completeText = ComplexDialogueText.AnalyzeText(text, RunLineLogger);

                foreach (string currentText in completeText.Parse())
                {
                    onLineUpdate?.Invoke(currentText);
                    if (proceedToNextLine)
                    {
                        // We've requested a skip of the entire line.
                        // Display all of the text immediately.
                        onLineUpdate?.Invoke(text);
                        break;
                    }
                    yield return new WaitForSeconds(textSpeed);
                }
            }
            else
            {
                // Display the entire line immediately if textSpeed <= 0
                onLineUpdate?.Invoke(text);
            }

            // We're now waiting for the player to move on to the next line
            proceedToNextLine = false;

            // Indicate to the rest of the game that the line has finished being delivered
            onLineFinishDisplaying?.Invoke();

            while (!proceedToNextLine)
            {
                yield return null;
            }

            // Avoid skipping lines if textSpeed == 0
            yield return new WaitForEndOfFrame();

            // Hide the text and prompt
            onLineEnd?.Invoke();

            onComplete();

        }

        public new void MarkLineComplete()
        {
            proceedToNextLine = true;
        }

        private void RunLineLogger(Exception ex)
        {
            Debug.LogException(ex);
        }
    }

}