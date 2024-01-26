# EASY NUMBER -

Now the problem of dealing with large numbers in your games is over. Thanks to Easy Number, you will be able to use large numbers easily.

# Set value from inspector
![](https://github.com/AtilganSak/ProjectImages/blob/main/Easy%20Number/Screenshot_2.png)

NOTE: If you leave the steps array empty, the value will be 0.

# Easy code usage
```C#
moneyText.text = easyNumber.ToString();        

easyNumber1 += easyNumber2;

easyNumber1 += 1000;

bool isGreate = easyNumber1 > easyNumber2;

bool isEqualGreate = easyNumber1 >= easyNumber2;
```

# Json save supported
```C#
PlayerDB.Instance.money = easyNumber1;
PlayerDB.Instance.Save();

easyNumber2 = PlayerDB.Instance.money;
```

#EXAMPLE

![](https://github.com/AtilganSak/ProjectImages/blob/main/Easy%20Number/GIF.gif)
