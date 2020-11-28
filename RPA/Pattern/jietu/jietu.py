import os
import sys
import cv2
import win32api
import win32con
import win32gui
import win32ui
import time

sys.path.append('.\\venv\\lib\\site-packages\\win32\\lib')
sys.path.append('.\\venv\\lib\\site-packages\\cv2\\lib')

# 全屏截图
'''
函数直接调用即可 无需参数
'''


def window_capture():
    hee = sys.getwindowsversion()[0]
    filename = 'wcappic\\'r''
    localtime = str(time.time())
    filename = filename + localtime
    filename = filename + '.png'
    f = open("wcappiclist.txt", "a")

    f.writelines(filename)
    f.writelines('\n')
    f.close()
    print(filename)
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
def capture(bbox):
    filename = 'cappic\\'
    localtime = str(time.time())
    filename = filename + localtime
    filename = filename + '.png'
    f = open("smacappiclist.txt", "a")

    f.writelines(filename)
    f.writelines('\n')
    f.close()
    print(filename)
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
    saveDC.BitBlt((bbox[0], bbox[1]), (bbox[2], bbox[3]), mfcDC, (0, 0), win32con.SRCCOPY)
    saveBitMap.SaveBitmapFile(saveDC, filename)
    return filename


'''
微信、qq点击使用
'''
def click_capture(x, y):
    hee = sys.getwindowsversion()[0]
    print('{0} at {1}'.format('Pressed', (x, y)))
    filename = 'smacappic\\'
    localtime = str(time.time())
    filename = filename + localtime
    filename = filename + '.png'
    f = open("smacappiclist.txt", "a")
    f.write(filename)
    f.write('\n')
    f.close()
    f2 = open("xyrecond.txt", "a")
    f2.write(x)
    f2.write("\n")
    f2.write(y)
    f2.write('\n')
    f2.close()
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
def dealallpic():
    f1 = open("smacappiclist.txt", "r")
    f2 = open("xyrecond.txt", "r")
    filename=f1.readline()
    while filename:
        x=f2.readline()
        y=f2.readline()
        dealpic(filename,x,y)
        filename =f1.readline()
    f1.close()
    f2.close()


'''
# 文档描述
每行一个文件名
wcappiclist.txt 对应 wcappic文件夹 存储全屏截图window_capture 截图
cappiclist.txt 对应cappic文件夹 存储局部截图capture截图
'''

