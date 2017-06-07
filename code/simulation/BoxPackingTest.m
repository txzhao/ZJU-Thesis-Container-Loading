%====================BoxPackingTest===================%
% read in data
[BoxAmount] = xlsread('BR1');
minBorderVar = 20;

[BoxStack,Border,dAreaRatio_best,dSpaceRatio_best,C_best, FinishedBox_best,minBlankPoints,...
    BorderVars,BoxPackingInfo_record] = BoxPacking(BoxAmount,minBorderVar);

imshow(1-C_best,[-2,1],'InitialMagnification','fit');
