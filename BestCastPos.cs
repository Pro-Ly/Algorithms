// See https://aka.ms/new-console-template for more information
using System.Collections.Generic;

Console.WriteLine("Hello, World!");

Circle cast = new Circle()
{
    pos = new Pos { x = 0.0, y = 0.0, },
    r = 3.0,
};

Circle[] enemies = new Circle[]
{
    new Circle { pos = new Pos { x = -6.5,   y = 2.0  },  r = 2.0 },
    new Circle { pos = new Pos { x = 4.0,   y = 7.0  },  r = 1.0 },
    new Circle { pos = new Pos { x = -8.0,  y = 1.5  },  r = 3.0 },
    new Circle { pos = new Pos { x = 9.0,   y = 2.0  },  r = 1.5 },
    new Circle { pos = new Pos { x = -6.6,  y = 3.5  },  r = 2.4 },
    new Circle { pos = new Pos { x = -2.0,  y = -8.8 },  r = 1.3 },
    new Circle { pos = new Pos { x = 12.0,  y = -0.7 },  r = 2.5 },
    new Circle { pos = new Pos { x = -4.7,  y = 15.0 },  r = 1.1 },
    new Circle { pos = new Pos { x = 5.5,   y = -6.8 },  r = 2.7 },
    new Circle { pos = new Pos { x = 1.1,   y = 11.0 },  r = 0.5 },
};

for (int i = 0; i < 10000; i++)
{
    //Solve(cast, 4, enemies, 3);
}
Pos ans = Solve(cast, 1, enemies, 2);
Console.WriteLine($"Pos.x = {ans.x} | Pos.y = {ans.y}");

Pos Solve(Circle cast, double spellRadius, Circle[] enemies, int accuracy)
{
    //第一步去掉距离超出施法距离+技能范围的enemy
    double maxDis = cast.r + spellRadius;
    int left = 0;
    int right = enemies.Length - 1;
    while (left <= right)
    {
        while (left <= right && Distance(enemies[right].pos, cast.pos) - enemies[right].r > maxDis)
        {
            right--;
        }

        while (left <= right && Distance(enemies[left].pos, cast.pos) - enemies[left].r <= maxDis)
        {
            left++;
        }
        if (left < right)
        {
            enemies[left] = enemies[right];
            right--;
        }
    }

    //第二步 选出最优点
    Pos ans = new Pos();
    int maxCount = 0;
    for (int i = 0; i <= right; i++)
    {
        //Console.WriteLine($"{enemies[i].pos.x}  {enemies[i].pos.y}");
        List<Arc> res = new List<Arc>();
        for (int j = 0; j <= right; j++)
        {
            if (i == j)
                continue;
            double dis = Distance(enemies[i].pos, enemies[j].pos);
            if (dis <= enemies[i].r + enemies[j].r + 2 * spellRadius)
            {
                double r1 = enemies[i].r + spellRadius;
                double r2 = enemies[j].r + spellRadius;
                double a = Math.Acos((r1 * r1 + dis * dis - r2 * r2) / (2 * r1 * dis));
                double b = Math.Atan2(enemies[j].pos.y - enemies[i].pos.y, enemies[j].pos.x - enemies[i].pos.x);
                res.Add(new Arc { angle = b - a, isStart = true });
                res.Add(new Arc { angle = b + a, isStart = false });
            }

        }
        res.Sort((Arc a, Arc b) => a.angle.CompareTo(b.angle));
        int curCount = 1;
        for (int j = 0; j < res.Count; j++)
        {
            if (res[j].isStart == true)
                curCount++;
            else
                curCount--;
            if (curCount > maxCount && j + 1 < res.Count)
            {
                double R = (enemies[i].r + spellRadius);
                double startX = enemies[i].pos.x + R * Math.Cos(res[j].angle);
                double startY = enemies[i].pos.y + R * Math.Sin(res[j].angle);
                double deltaX = R * (Math.Cos(res[j + 1].angle) - Math.Cos(res[j].angle)) / accuracy;
                double deltaY = R * (Math.Sin(res[j + 1].angle) - Math.Sin(res[j].angle)) / accuracy;
                for (int k = 1; k < accuracy; k++)
                {
                    double x = startX + k * deltaX;
                    double y = startY + k * deltaY;
                    double dis = Math.Sqrt((x - cast.pos.x) * (x - cast.pos.x) + (y - cast.pos.y) * (y - cast.pos.y));
                    if (dis <= cast.r)
                    {
                        maxCount = curCount;
                        ans.x = x;
                        ans.y = y;
                        break;
                    }
                }
            }
        }
        if (maxCount == 0)
        {
            double tempDis = Distance(cast.pos, enemies[i].pos);
            if (cast.r + spellRadius + enemies[i].r >= tempDis)
            {
                maxCount = 1;
                ans.x = enemies[i].pos.x - enemies[i].r / tempDis * (enemies[i].pos.x - cast.pos.x);
                ans.y = enemies[i].pos.y - enemies[i].r / tempDis * (enemies[i].pos.y - cast.pos.y);
            }
        }
    }

    //连一个enemy都碰不到的情况
    return ans;
}

static double Distance(Pos a, Pos b)
{
    return Math.Sqrt((a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y));
}

struct Circle
{
    public double r;
    public Pos pos;
}

struct Pos
{
    public double x;
    public double y;
}

struct Arc
{
    public double angle;
    public bool isStart;
}



