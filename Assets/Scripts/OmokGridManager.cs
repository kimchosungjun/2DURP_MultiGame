using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OmokStoneEnum;

public class OmokGridManager : MonoBehaviour
{
    Dictionary<Vector2Int, OmokStone> gridGroup = new Dictionary<Vector2Int, OmokStone>();

    public bool CanPutStone(Vector2Int position, StoneColor color)
    {
        // �ٵ��� ������ ����� ��
        if (!CheckGridRange(position))
            return false;

        if (color == StoneColor.Black)
        {
            return true;
        }
        else
        {
            // �鵹�� ���� �� �ƹ��� ������ ����.
            if (gridGroup.ContainsKey(position))
                return false;
            return true;
        }
    }

    public void PutStone(Vector2Int position, StoneColor color)
    {

    }

    public bool CheckGridRange(Vector2Int position)
    {
        int xPos = Mathf.Abs(position.x);
        int yPos = Mathf.Abs(position.y);

        if (xPos >= 9 || yPos >= 9)
            return false;
        return true;
    }
}
