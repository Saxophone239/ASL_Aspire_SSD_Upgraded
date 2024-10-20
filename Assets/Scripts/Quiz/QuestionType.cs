using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * As described on the Figma file, the question types are:
 * 
 *  ASL Sign            ->  English Word
 *  English Definition  ->  English Word
 *  English Word        ->  English Definition
 *  ASL Definition      ->  English Definition
 *  Icon                ->  English Word
 */
public enum QuestionType
{
    // SignWordToWord,
    DefToWord,
    WordToDef,
    // SignDefToDef,
    // IconToWord
}
