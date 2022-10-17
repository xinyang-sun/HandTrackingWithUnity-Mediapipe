import math

import cv2
import numpy as np
from cvzone.HandTrackingModule import HandDetector
import socket
import mediapipe as mp

import tkinter as tk

switch = False
port = 5056

# Parameters
width, height = 1280, 720

# Webcam
cap = cv2.VideoCapture(2)
cap.set(3, width)
cap.set(4, height)

# Hand Detector
detector = HandDetector(maxHands=1, detectionCon=0.8)

# Communication
sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)


x = [300, 245, 200, 170, 145, 130, 112, 103, 93, 87, 80, 75, 70, 67, 62, 59, 57]
y = [20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80, 85, 90, 95, 100]
coff = np.polyfit(x, y, 2)
d1=0

# GUI
window = tk.Tk()
window.title('tracking hand')
window.geometry('400x200')
labelP = tk.Label(window, text='port: ').place(x=150, y=0)
var = tk.IntVar()
var.set(port)
label = tk.Label(window, textvariable=var)
label.pack()
entery = tk.Entry(window, show=None)
entery.pack()

def insertPort():
    global port
    port = entery.get()
    port = int(port)
    var.set(port)

def changeSwitch():
    serverAddressPort = ("127.0.0.1", port)
    switch = True
    while switch:
        success, img = cap.read()

        # Hands
        hands, img = detector.findHands(img)
        data = []
        # Landmark values
        if hands:
            hand = hands[0]  # get first hand detected
            lmList = hand['lmList']  # Get the landmark list
            handType = hand['type']  # Get the hand type

            x1, y1 = lmList[5][:2]
            x2, y2 = lmList[17][:2]
            distance = int(math.sqrt((y2 - y1) ** 2 + (x2 - x1) ** 2))
            A, B, C = coff
            distanceCM = A * distance ** 2 + B * distance + C
            # print(distanceCM)

            print(port)
            for lm in lmList:
                data.extend([lm[0], height - lm[1], lm[2], distanceCM])
            print(data)
            sock.sendto(str.encode(str(data)), serverAddressPort)

        img = cv2.resize(img, (0, 0), None, 0.5, 0.5)
        cv2.imshow("Image", img)
        cv2.waitKey(1)
        if cv2.getWindowProperty('Image', cv2.WND_PROP_VISIBLE) < 1:
            switch = False


button = tk.Button(window,text='Enter', width=15, height=2, command=insertPort)
button.pack()
button = tk.Button(window,text='Start', width=15, height=2, command=changeSwitch)
button.pack()
window.mainloop()


