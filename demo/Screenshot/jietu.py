import os
import sys
import cv2
import win32api
import win32con
import win32gui
import win32ui

# 全屏截图
'''
函数直接调用即可 无需参数
'''


def capture(dir,namePrefix):
    hee = sys.getwindowsversion()[0]

    filename = dir
    if not os.path.exists(filename):
        os.mkdir(filename)
    filename = filename +"\\"+ namePrefix
    filename = filename + '.png'

    hwnd = 0  # 窗口的编号，0号表示当前活跃窗口
    # 根据窗口句柄获取窗口的设备上下文DC（Divice Context）
    hwndDC = win32gui.GetWindowDC(hwnd)
    # 根据窗口的DC获取mfcDC
    mfcDC = win32ui.CreateDCFromHandle(hwndDC)
    # mfcDC创建可兼容的DC
    saveDC = mfcDC.CreateCompatibleDC()
    # 创建bigmap准备保存图片
    saveBitMap = win32ui.CreateBitmap()
    # 获取监控器信息
    MoniterDev = win32api.EnumDisplayMonitors(None, None)
    w = MoniterDev[0][2][2]
    h = MoniterDev[0][2][3]
    if hee == 10:
        w = int(1.25 * w)
        h = int(1.25 * h)
    # print w,h　　　#图片大小
    # 为bitmap开辟空间
    saveBitMap.CreateCompatibleBitmap(mfcDC, w, h)
    # 高度saveDC，将截图保存到saveBitmap中
    saveDC.SelectObject(saveBitMap)
    # 截取从左上角（0，0）长宽为（w，h）的图片
    saveDC.BitBlt((0, 0), (w, h), mfcDC, (0, 0), win32con.SRCCOPY)
    saveBitMap.SaveBitmapFile(saveDC, filename)
    return filename


# 局部截图

# bbox = (1849, 350, 1910, 850)
# 参数说明
# 第一个参数 开始截图的x坐标
# 第二个参数 开始截图的y坐标
# 第三个参数 x截取长度
# 第四个参数 y截取长度
def window_capture(dir,namePrefix,left,top,right,down):
    filename = dir
    if not os.path.exists(filename):
        os.mkdir(filename)
    filename = filename +"\\"+ namePrefix
    filename = filename + '.png'

    hwnd = 0  # 窗口的编号，0号表示当前活跃窗口
    # 根据窗口句柄获取窗口的设备上下文DC（Divice Context）
    hwndDC = win32gui.GetWindowDC(hwnd)
    # 根据窗口的DC获取mfcDC
    mfcDC = win32ui.CreateDCFromHandle(hwndDC)
    # mfcDC创建可兼容的DC
    saveDC = mfcDC.CreateCompatibleDC()
    # 创建bigmap准备保存图片
    saveBitMap = win32ui.CreateBitmap()
    # 获取监控器信息
    MoniterDev = win32api.EnumDisplayMonitors(None, None)
    w = MoniterDev[0][2][2]
    h = MoniterDev[0][2][3]
    # print w,h　　　#图片大小
    # 为bitmap开辟空间
    saveBitMap.CreateCompatibleBitmap(mfcDC, w, h)
    # 高度saveDC，将截图保存到saveBitmap中
    saveDC.SelectObject(saveBitMap)
    saveDC.BitBlt((left, top), (right-left, down-top), mfcDC, (0, 0), win32con.SRCCOPY)
    saveBitMap.SaveBitmapFile(saveDC, filename)
    return filename


'''
微信、qq点击使用
'''


def click_capture(dir,namePrefix,x, y):
    hee = sys.getwindowsversion()[0]
    filename = dir
    if not os.path.exists(filename):
        os.mkdir(filename)
    filename = filename +"\\"+ namePrefix
    filename = filename + '.png'

    hwnd = 0  # 窗口的编号，0号表示当前活跃窗口
    # 根据窗口句柄获取窗口的设备上下文DC（Divice Context）
    hwndDC = win32gui.GetWindowDC(hwnd)
    # 根据窗口的DC获取mfcDC
    mfcDC = win32ui.CreateDCFromHandle(hwndDC)
    # mfcDC创建可兼容的DC
    saveDC = mfcDC.CreateCompatibleDC()
    # 创建bigmap准备保存图片
    saveBitMap = win32ui.CreateBitmap()
    # 获取监控器信息
    MoniterDev = win32api.EnumDisplayMonitors(None, None)
    w = MoniterDev[0][2][2]
    h = MoniterDev[0][2][3]
    if hee == 10:
        w = int(1.25 * w)
        h = int(1.25 * h)

    # 图片大小
    # 为bitmap开辟空间
    saveBitMap.CreateCompatibleBitmap(mfcDC, w, h)
    # 高度saveDC，将截图保存到saveBitmap中
    saveDC.SelectObject(saveBitMap)
    # 截取从左上角（0，0）长宽为（w，h）的图片
    saveDC.BitBlt((0, 0), (w, h), mfcDC, (0, 0), win32con.SRCCOPY)
    saveBitMap.SaveBitmapFile(saveDC, filename)
    return filename


def dealpic(filename, x, y):
    img = cv2.imread(filename)
    img_gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
    print(img_gray[y][x])
    a = {}
    list1 = []
    print(x, y)
    for i in range(x - 5, x + 5, 1):
        # print(i)
        for j in range(y - 5, y + 5, 1):
            list1.append((j, i))
            if tuple(img[j][i]) not in a:
                a[tuple(img[j][i])] = 1
            else:
                a[tuple(img[j][i])] += 1
    res = max(a, key=lambda ste: a[ste])
    print(res)
    # print(list1)
    print("1")
    listpoint = []
    lowx = x
    lowy = y
    highx = x
    highy = y
    res1 = (res[0] - 3, res[1] - 3, res[2] - 3)
    res2 = (res[0] + 3, res[1] + 3, res[2] + 3)
    for h in list1:
        if res1 <= tuple(img[h[0]][h[1]]) <= res2:
            lowx = min(lowx, h[1])
            lowy = min(lowy, h[0])
            highx = max(highx, h[1])
            highy = max(highy, h[0])
    print(lowx, highx, lowy, highy, "1")
    listpoint.append((lowy, lowx))
    listpoint.append((lowy, highx))
    listpoint.append((highy, lowx))
    listpoint.append((highy, highx))
    print(listpoint)
    while listpoint:
        temp1 = listpoint[0]
        # print(temp1)
        for i in (-1, 0, 1):
            for j in (-1, 0, 1):
                if (temp1[0] + i, temp1[1] + j) not in list1:
                    # print(res1 <= tuple(img[temp1[0] + i][temp1[1] + j]) <= res2)
                    if res1 <= tuple(img[temp1[0] + i][temp1[1] + j]) <= res2:
                        listpoint.append((temp1[0] + i, temp1[1] + j))
                        lowx = min(lowx, temp1[1] + j)
                        lowy = min(lowy, temp1[0] + i)
                        highx = max(highx, temp1[1] + j)
                        highy = max(highy, temp1[0] + i)
                        list1.append((temp1[0] + i, temp1[1] + j))
        listpoint.pop(0)
    print(lowx, highx, lowy, highy, "2")
    last = img[lowy:highy - 1, lowx:highx - 1]
    os.remove(filename)
    cv2.imwrite(filename, last)
    return filename


'''
点击截图后面的处理
'''

'''
def dealallpic():
    f1 = open("text\\smacappiclist.txt", "r")
    f2 = open("text\\xyrecond.txt", "r")
    filename = f1.readline()
    while filename:
        x = f2.readline()
        y = f2.readline()
        dealpic(filename, x, y)
        filename = f1.readline()
    f1.close()
    f2.close()
'''

'''
# 文档描述
每行一个文件名
wcappiclist.txt 对应 wcappic文件夹 存储全屏截图window_capture 截图
cappiclist.txt 对应cappic文件夹 存储局部截图capture截图
'''

'''
def dealpic1(filename, x, y):
    print(filename)
    img = cv2.imread(filename)
    a = {}
    list1 = []
    print(x, y)
    for i in range(x - 5, x + 5, 1):
        # print(i)
        for j in range(y - 5, y + 5, 1):
            list1.append((j, i))
            if tuple(img[j][i]) not in a:
                a[tuple(img[j][i])] = 1
            else:
                a[tuple(img[j][i])] += 1
    res = max(a, key=lambda ste: a[ste])
    # time = a[res]
    print(1)
    
    if time < 30:
        x += 100
        
        for i in range(x - 5, x + 5, 1):
            # print(i)
            for j in range(y - 5, y + 5, 1):
                if tuple(img[j][i]) not in a:
                    a[tuple(img[j][i])] = 1
                else:
                    a[tuple(img[j][i])] += 1
        res = max(a, key=lambda ste: a[ste])
    # print(list1)
    
    print(2)
    lowx = x
    lowy = y
    highx = x
    highy = y
    res1 = (res[0] - 3, res[1] - 3, res[2] - 3)
    res2 = (res[0] + 3, res[1] + 3, res[2] + 3)
    while res1 <= tuple(img[y][lowx]) <= res2:
        lowx -= 1
    while res1 <= tuple(img[y][highx]) <= res2:
        highx += 1
    while res1 <= tuple(img[lowy][x]) <= res2:
        lowy -= 1
    while res1 <= tuple(img[highy][x]) <= res2:
        highy += 1
    print(3)
    while 1:
        sign = True
        print((lowy, highy))
        for i in range(lowy, highy):
            print((lowy, highy))
            if res1 <= tuple(img[i][lowx]) <= res2:
                lowx -= 1
                sign = False
        for i in range(lowy, highy):
            if res1 <= tuple(img[i][highx]) <= res2:
                highx += 1
                sign = False
        for i in range(lowx, highx):
            if res1 <= tuple(img[lowy][i]) <= res2:
                lowy -= 1
                sign = False
        for i in range(lowx, highx):
            if res1 <= tuple(img[highy][i]) <= res2:
                highy += 1
                sign = False
        if sign:
            break
    print(4)
    print(lowx, highx, lowy, highy, "2")
    last = img[lowy:highy - 1, lowx:highx - 1]
    os.remove(filename)
    cv2.imwrite(filename, last)
    return filename


filena = window_capture()
dealpic1(filena, 100, 200)
'''
