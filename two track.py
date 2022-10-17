import math

import cv2
import numpy as np
from cvzone.HandTrackingModule import HandDetector
import socket
import mediapipe as mp

# Parameters
width, height = 1280, 720

# Webcam
cap = cv2.VideoCapture(2)
cap.set(3, width)
cap.set(4, height)

# Hand Detector
detector = HandDetector(maxHands=2, detectionCon=0.8)

# Communication
sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
serverAddressPort = ("127.0.0.1", 5053)

x = [300, 245, 200, 170, 145, 130, 112, 103, 93, 87, 80, 75, 70, 67, 62, 59, 57]
y = [20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80, 85, 90, 95, 100]
coff = np.polyfit(x, y, 2)

while True:
    success, img = cap.read()

    # Hands
    hands, img = detector.findHands(img)
    data = []
    lmList = []
    lmList1 = []
    HandLeft = []
    HandRight = []
    # Landmark values
    if hands:
        hand=hands[0] # get first hand detected
        handType = hand['type'] # Get the hand type
        lmList.append(handType)
        lmList.append(hand['lmList']) # Get the landmark list

        if lmList[0] == "Left" and len(hands) == 1:
            HandLeft = hand['lmList']
        else:
            HandRight = hand['lmList']

        if len(hands) == 2:
            hand1=hands[1]
            hand1Type = hand1['type']
            lmList1.append(hand1Type)
            lmList1.append(hand1['lmList'])

            if lmList1[0] == "Left":
                HandLeft = hand1['lmList']
                HandRight = hand['lmList']
            else:
                HandLeft = hand['lmList']
                HandRight = hand1['lmList']




        if len(HandLeft) != 0 and len(HandRight) != 0:
            Lx1, Ly1 = HandLeft[5][:2]
            Lx2, Ly2 = HandLeft[17][:2]
            Ldistance = int(math.sqrt((Ly2-Ly1)**2+(Lx2-Lx1)**2))
            A, B, C = coff
            LdistanceCM = A * Ldistance ** 2 + B * Ldistance + C

            Rx1, Ry1 = HandRight[5][:2]
            Rx2, Ry2 = HandRight[17][:2]
            Rdistance = int(math.sqrt((Ry2 - Ry1) ** 2 + (Rx2 - Rx1) ** 2))
            A, B, C = coff
            RdistanceCM = A * Rdistance ** 2 + B * Rdistance + C
        elif len(HandLeft) != 0 and len(HandRight) == 0:
            Lx1, Ly1 = HandLeft[5][:2]
            Lx2, Ly2 = HandLeft[17][:2]
            Ldistance = int(math.sqrt((Ly2 - Ly1) ** 2 + (Lx2 - Lx1) ** 2))
            A, B, C = coff
            LdistanceCM = A * Ldistance ** 2 + B * Ldistance + C
        elif len(HandLeft) == 0 and len(HandRight) != 0:
            Rx1, Ry1 = HandRight[5][:2]
            Rx2, Ry2 = HandRight[17][:2]
            Rdistance = int(math.sqrt((Ry2 - Ry1) ** 2 + (Rx2 - Rx1) ** 2))
            A, B, C = coff
            RdistanceCM = A * Rdistance ** 2 + B * Rdistance + C
        #print(LdistanceCM, RdistanceCM)

        if len(HandLeft) != 0:
            data.extend(["Left"])
        for lm in HandLeft:
            data.extend([lm[0], height - lm[1], lm[2], LdistanceCM])
        if len(HandRight) != 0:
            data.extend(["Right"])
        for lm1 in HandRight:
            data.extend([lm1[0], height - lm1[1], lm1[2], RdistanceCM])
        print(data)
        sock.sendto(str.encode(str(data)), serverAddressPort)

    img = cv2.resize(img, (0, 0), None, 0.5, 0.5)
    cv2.imshow("Image", img)
    cv2.waitKey(1)