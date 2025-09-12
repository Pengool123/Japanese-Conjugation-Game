using System.Collections.Generic;
using System.Diagnostics.Tracing;
namespace jp_conj_game.scripts;
using Godot;

public class OnScreenJPText
{
    private List<Label> _labels;
    
    //x y is the middle of the string
    public List<Label> JpLabel(Word word,
        int x = 0,
        int y = 0,
        int size = 100)
    {
        _labels = new List<Label>();
        string noFuri = word.noFurigana;
        string fullWord = word.wordLine;
        int length = noFuri.Length;
        //go to the top left of the string for furigana text
        int startX = x - length * size / 2;
        int startY = y + (size / 2);
        string tempFuriganaText = "";
        
        GD.Print($"no furi is {noFuri} | full word is {fullWord}");
        GD.Print($"start x = {startX} |  start y = {startY}");

        float currX = startX;
        float currY = startY;
        int kanjiCount = 0;
        bool furiganaStart = false;
        
        foreach (char i in fullWord)
        {
            switch (i)
            {
                case '[':
                    furiganaStart = true;
                    break;
                case ']':
                    GD.Print("starting furigana");
                    furiganaStart = false;
                    if (kanjiCount > 1)
                    {
                        tempFuriganaText = string.Join(" ", tempFuriganaText.ToCharArray());
                    }
                    int furiganaSize = size / 4;
                    //go to the center of the kanji/kanjis
                    float furiganaX = currX -
                                    ((float)kanjiCount * size / 2) //center of the kanji
                                    - ((float)tempFuriganaText.Length / 2 * furiganaSize); //move bc of top left offset
                                    
                    float furiganaY = currY - furiganaSize;
                    
                    Label furiLabel =  new Label();
                    furiLabel.Text = tempFuriganaText;
                    furiLabel.Position = new Vector2(furiganaX, furiganaY);
                    furiLabel.Size = new Vector2(furiganaSize * tempFuriganaText.Length, furiganaSize);
                    furiLabel.AddThemeFontSizeOverride("font_size",furiganaSize);
                    _labels.Add(furiLabel);
                    
                    tempFuriganaText = "";
                    kanjiCount = 0;
                    break;
                default:
                    if (!furiganaStart)
                    {
                        Label label = new Label();
                        label.Text = i.ToString();
                        label.Position = new Vector2(currX, currY);
                        label.Size = new Vector2(size, size);
                        label.AddThemeFontSizeOverride("font_size",size);
                        _labels.Add(label);

                        kanjiCount = word.isGana(i) ? 0 : kanjiCount+1;
                        currX += size;
                    }
                    else
                    {
                        tempFuriganaText += i;
                    }
                    break;
            }
        }
        
        return _labels;
    }
}