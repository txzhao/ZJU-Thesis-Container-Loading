%====================BoxPackingTest===================%
% [BoxAmount] = xlsread('data_empty');
[BoxAmount] = xlsread('BR1');
minBorderVar = 20;
[BoxStack,Border,dAreaRatio_best,dSpaceRatio_best,C_best, FinishedBox_best,minBlankPoints,...
    BorderVars,BoxPackingInfo_record] = BoxPacking3(BoxAmount,minBorderVar);
% [dAreaRatio_best,C_best, FinishedBox_best,minBlankPoints,BoxPackingInfo_record] = BoxPacking2(BoxAmount);
imshow(1-C_best,[-2,1],'InitialMagnification','fit');