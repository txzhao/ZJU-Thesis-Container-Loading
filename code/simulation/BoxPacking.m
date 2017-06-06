function [dAreaRatio_best, C_best, FinishedBox_best, unFinishBoxNo_best] = BoxPacking(WBox, HBox, BoxSize)
% 
% WBox Ϊ���ռ��ȣ���������
% HBox Ϊ���ռ�߶ȣ���������
% BoxSize Ϊ��Ҫ���ľ��ο����ߣ���Ϊ��������
numBox = length(BoxSize);
if size(BoxSize,1) < numBox
    BoxSize = BoxSize';
end

% ��ʼ�����: �߽�Ϊ2, �м�Ϊ0(��ʾΪ��)
C0 = zeros(HBox+2, WBox+2);
C0(1,:) = 2*ones(1,WBox+2);
C0(HBox+2,:) = C0(1,:);
C0(:,1) = 2*ones(HBox+2,1);
C0(:,WBox+2) = C0(:,1);

dAreaRatio_best = 0;
for k=1:numBox
    % �Ե�k��ģ��Ϊ��һ�飨δת�ã��������Ͻ�
    [dAreaRatio, C, FinishedBox, unFinishBoxNo] = OptiBoxPacking(k, 0, C0, WBox, HBox, BoxSize);
%     k, dAreaRatio
    if dAreaRatio > dAreaRatio_best
        dAreaRatio_best = dAreaRatio;
        C_best = C;
        FinishedBox_best = FinishedBox;
        unFinishBoxNo_best = unFinishBoxNo;
    end
    % �Ե�k��ģ��Ϊ��һ�飨��ת�ã��������Ͻ�
    [dAreaRatio, C, FinishedBox, unFinishBoxNo] = OptiBoxPacking(k, 1, C0, WBox, HBox, BoxSize);
    if dAreaRatio > dAreaRatio_best
        dAreaRatio_best = dAreaRatio;
        C_best = C;
        FinishedBox_best = FinishedBox;
        unFinishBoxNo_best = unFinishBoxNo;
    end
end


%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
function [dAreaRatio, C, FinishedBox, unFinishBoxNo] = OptiBoxPacking(firstBoxNo, bTranspose, C0, WBox, HBox, BoxSize)
C = C0;
numBox = length(BoxSize);
% ����δ�����������鼯��
unFinishBox = 1:numBox; 
FinishedBox = zeros(numBox,5); numFinishedBox = 0;

% �����Ͻǣ�2��2��Ϊ�ǣ����õ�һ����ο�, ������ģ���δ��伯����������伯��
unFinishBox(firstBoxNo) = 0;
numFinishedBox = numFinishedBox + 1;
FinishedBox(numFinishedBox,1) = firstBoxNo;
FinishedBox(numFinishedBox,2) = 2; % ��¼��ģ������Ͻ�����
FinishedBox(numFinishedBox,3) = 2;
width_firstBox = BoxSize(firstBoxNo,1);
height_firstBox = BoxSize(firstBoxNo,2);
if bTranspose
    fTemp = width_firstBox;
    width_firstBox = height_firstBox;
    height_firstBox = fTemp;
end
FinishedBox(numFinishedBox,4) = width_firstBox; % ��¼��ģ��Ŀ��
FinishedBox(numFinishedBox,5) = height_firstBox; % ��¼��ģ��ĸ߶�

% �����Ͻǣ�2��2��Ϊ�ǣ����þ��ο鲢��ע�߽�
C(2:(height_firstBox+1),2:(width_firstBox+1)) = 1;
C(2,2:(width_firstBox+1)) = 2;
C(height_firstBox+1,2:(width_firstBox+1)) = 2;
C(2:(height_firstBox+1),2) = 2;
C(2:(height_firstBox+1),width_firstBox+1) = 2;

for k = 2:numBox
    % ���㵱ǰ����µ�ÿһ���ս�
    numAngle = 0; AnglePara = [];
    for i=2:HBox+1
        for j=2:WBox+1
            if C(i,j)<0.1 % ����Ϊ�յ�
                if C(i-1,j) + C(i+1,j) + C(i,j-1) + C(i,j+1) > 3.9
                    % ��¼�ս�����
                    numAngle = numAngle + 1;
                    AnglePara(numAngle,1) = i; AnglePara(numAngle,2) = j;
                    if C(i,j-1) + C(i+1,j) > 3.9
                        AnglePara(numAngle,3) = 1;  % ��λ�ڵ�I����
                    elseif C(i,j+1) + C(i+1,j) > 3.9
                        AnglePara(numAngle,3) = 2;  % ��λ�ڵ�II����
                    elseif C(i,j+1) + C(i-1,j) > 3.9
                        AnglePara(numAngle,3) = 3;  % ��λ�ڵ�III����
                    elseif C(i,j-1) + C(i-1,j) > 3.9
                        AnglePara(numAngle,3) = 4;  % ��λ�ڵ�IV����                        
                    end
                end
            end
        end
    end
    
    % ����ÿһ���սǣ�Ѱ����Կ�Ѩ����С�ľ��ο�
    HoleDensity = []; 
    for k1 = 1:numAngle
        y0 = AnglePara(k1,1); x0 = AnglePara(k1,2); nAngleType = AnglePara(k1,3); 
        dmin_Best = 100; IndexBestBox = 0; 
        switch nAngleType
            case 1
                for j =1:numBox
                    j1 = unFinishBox(j);
                    if j1>0 % ����δ����
                        widthBox = BoxSize(j1,1); heightBox = BoxSize(j1,2);
                        dTemp(1) = WBox + 1 - (widthBox + x0 - 1);
                        dTemp(2) = y0 - 1 - heightBox;
                        if dTemp(1)>=0 && dTemp(2)>=0 % δ���������߽�
                            % ������δ������룬������������Ƿ��ص�
                            YIndex = (y0 - heightBox+1):y0;
                            XIndex = x0:(x0 + widthBox - 1);
                            if sum(C(YIndex,XIndex))< 0.5 
                                % ���ںϷ�ռ�ǲ���,���������������С�����پ���
                                [dDist, numStickSide] = ShadowDistance(widthBox, heightBox, x0, y0, nAngleType, C, WBox, HBox);
                                minFunc = (dDist*dDist)/(BoxSize(j1,1)*BoxSize(j1,2));
                                if minFunc<dmin_Best % �������Ž��
                                    dmin_Best = minFunc;
                                    IndexBestBox = j1; 
                                    dDist_BestBox = dDist;
                                    numStickSide_BestBox = numStickSide;
                                    widthBestBox = widthBox; heightBestBox = heightBox; 
                                end
                            end
                        end
                        % ��δ����ת����ظ�����
                        widthBox = BoxSize(j1,2); heightBox = BoxSize(j1,1);
                        dTemp(1) = WBox + 1 - (widthBox + x0 - 1);
                        dTemp(2) = y0 - 1 - heightBox;
                        if dTemp(1)>=0 && dTemp(2)>=0 % δ���������߽�
                            % ������δ������룬������������Ƿ��ص�
                            YIndex = (y0 - heightBox+1):y0;
                            XIndex = x0:(x0 + widthBox - 1);
                            if sum(C(YIndex,XIndex))< 0.5 
                                % ���ںϷ�ռ�ǲ���,���������������С�����پ���
                                [dDist, numStickSide] = ShadowDistance(widthBox, heightBox, x0, y0, nAngleType, C, WBox, HBox);
                                minFunc = (dDist*dDist)/(BoxSize(j1,1)*BoxSize(j1,2));
                                if minFunc<dmin_Best % �������Ž��
                                    dmin_Best = minFunc;
                                    IndexBestBox = j1; 
                                    dDist_BestBox = dDist;
                                    numStickSide_BestBox = numStickSide;
                                    widthBestBox = widthBox; heightBestBox = heightBox; 
                                end
                            end
                        end
                    end
                end
                
            case 2
                for j =1:numBox
                    j1 = unFinishBox(j);
                    if j1>0 % ����δ����
                        widthBox = BoxSize(j1,1); heightBox = BoxSize(j1,2);
                        dTemp(1) = x0 - 1 - widthBox;
                        dTemp(2) = y0 - 1 - heightBox;
                        if dTemp(1)>=0 && dTemp(2)>=0 % δ���������߽�
                            % ������δ������룬������������Ƿ��ص�
                            YIndex = (y0 - heightBox+1):y0;
                            XIndex = (x0 - widthBox+1):x0;
                            if sum(C(YIndex,XIndex))< 0.5 
                                % ���ںϷ�ռ�ǲ���,���������������С�����پ���
                                [dDist, numStickSide] = ShadowDistance(widthBox, heightBox, x0, y0, nAngleType, C, WBox, HBox);
                                minFunc = (dDist*dDist)/(BoxSize(j1,1)*BoxSize(j1,2));
                                if minFunc<dmin_Best % �������Ž��
                                    dmin_Best = minFunc;
                                    IndexBestBox = j1;
                                    dDist_BestBox = dDist;
                                    numStickSide_BestBox = numStickSide;
                                    widthBestBox = widthBox; heightBestBox = heightBox; 
                                end
                            end
                        end
                        % ��δ����ת����ظ�����                        
                        widthBox = BoxSize(j1,2); heightBox = BoxSize(j1,1);
                        dTemp(1) = x0 - 1 - widthBox;
                        dTemp(2) = y0 - 1 - heightBox;
                        if dTemp(1)>=0 && dTemp(2)>=0 % δ���������߽�
                            % ������δ������룬������������Ƿ��ص�
                            YIndex = (y0 - heightBox+1):y0;
                            XIndex = (x0 - widthBox+1):x0;
                            if sum(C(YIndex,XIndex))< 0.5 
                                % ���ںϷ�ռ�ǲ���,���������������С�����پ���
                                [dDist, numStickSide] = ShadowDistance(widthBox, heightBox, x0, y0, nAngleType, C, WBox, HBox);
                                minFunc = (dDist*dDist)/(BoxSize(j1,1)*BoxSize(j1,2));
                                if minFunc<dmin_Best % �������Ž��
                                    dmin_Best = minFunc;
                                    IndexBestBox = j1;
                                    dDist_BestBox = dDist;
                                    numStickSide_BestBox = numStickSide;
                                    widthBestBox = widthBox; heightBestBox = heightBox; 
                                end
                            end
                        end
                    end
                end
                
            case 3
                for j =1:numBox
                    j1 = unFinishBox(j);
                    if j1>0 % ����δ����
                        widthBox = BoxSize(j1,1); heightBox = BoxSize(j1,2);
                        dTemp(1) = x0 - widthBox - 1;
                        dTemp(2) = HBox + 1 -(y0 + heightBox -1);
                        if dTemp(1)>=0 && dTemp(2)>=0 % δ���������߽�
                            % ������δ������룬������������Ƿ��ص�
                            YIndex = y0:(y0+heightBox-1);
                            XIndex = (x0-widthBox+1):x0;
                            if sum(C(YIndex,XIndex))< 0.5 
                                % ���ںϷ�ռ�ǲ���,���������������С�����پ���
                                [dDist, numStickSide] = ShadowDistance(widthBox, heightBox, x0, y0, nAngleType, C, WBox, HBox);
                                minFunc = (dDist*dDist)/(BoxSize(j1,1)*BoxSize(j1,2));
                                if minFunc<dmin_Best % �������Ž��
                                    dmin_Best = minFunc;
                                    IndexBestBox = j1;
                                    dDist_BestBox = dDist;
                                    numStickSide_BestBox = numStickSide;
                                    widthBestBox = widthBox; heightBestBox = heightBox; 
                                end
                            end
                        end
                        % ��δ����ת����ظ�����
                        widthBox = BoxSize(j1,2); heightBox = BoxSize(j1,1);
                        dTemp(1) = x0 - widthBox - 1;
                        dTemp(2) = HBox + 1 -(y0 + heightBox -1);
                        if dTemp(1)>=0 && dTemp(2)>=0 % δ���������߽�
                            % ������δ������룬������������Ƿ��ص�
                            YIndex = y0:(y0+heightBox-1);
                            XIndex = (x0-widthBox+1):x0;
                            if sum(C(YIndex,XIndex))< 0.5 
                                % ���ںϷ�ռ�ǲ���,���������������С�����پ���
                                [dDist, numStickSide] = ShadowDistance(widthBox, heightBox, x0, y0, nAngleType, C, WBox, HBox);
                                minFunc = (dDist*dDist)/(BoxSize(j1,1)*BoxSize(j1,2));
                                if minFunc<dmin_Best % �������Ž��
                                    dmin_Best = minFunc;
                                    IndexBestBox = j1;
                                    dDist_BestBox = dDist;
                                    numStickSide_BestBox = numStickSide;
                                    widthBestBox = widthBox; heightBestBox = heightBox; 
                                end
                            end
                        end
                    end
                end

            case 4
                for j =1:numBox
                    j1 = unFinishBox(j);
                    if j1>0 % ����δ����
                        widthBox = BoxSize(j1,1); heightBox = BoxSize(j1,2);
                        dTemp(1) = WBox + 1 - (widthBox + x0 - 1);
                        dTemp(2) = HBox + 1 -(y0 + heightBox - 1);
                        if dTemp(1)>=0 && dTemp(2)>=0 % δ���������߽�
                            % ������δ������룬������������Ƿ��ص�
                            YIndex = y0:(y0+heightBox-1);
                            XIndex = x0:(x0+widthBox-1);
                            if sum(C(YIndex,XIndex))< 0.5 
                                % ���ںϷ�ռ�ǲ���,���������������С�����پ���
                                [dDist, numStickSide] = ShadowDistance(widthBox, heightBox, x0, y0, nAngleType, C, WBox, HBox);
                                minFunc = (dDist*dDist)/(BoxSize(j1,1)*BoxSize(j1,2));
                                if minFunc<dmin_Best % �������Ž��
                                    dmin_Best = minFunc;
                                    IndexBestBox = j1;
                                    dDist_BestBox = dDist;
                                    numStickSide_BestBox = numStickSide;
                                    widthBestBox = widthBox; heightBestBox = heightBox; 
                                end
                            end
                        end
                        % ��δ����ת����ظ�����
                        widthBox = BoxSize(j1,2); heightBox = BoxSize(j1,1);
                        dTemp(1) = WBox + 1 - (widthBox + x0 - 1);
                        dTemp(2) = HBox + 1 -(y0 + heightBox - 1);
                        if dTemp(1)>=0 && dTemp(2)>=0 % δ���������߽�
                            % ������δ������룬������������Ƿ��ص�
                            YIndex = y0:(y0+heightBox-1);
                            XIndex = x0:(x0+widthBox-1);
                            if sum(C(YIndex,XIndex))< 0.5 
                                % ���ںϷ�ռ�ǲ���,���������������С�����پ���
                                [dDist, numStickSide] = ShadowDistance(widthBox, heightBox, x0, y0, nAngleType, C, WBox, HBox);
                                minFunc = (dDist*dDist)/(BoxSize(j1,1)*BoxSize(j1,2));
                                if minFunc<dmin_Best % �������Ž��
                                    dmin_Best = minFunc;
                                    IndexBestBox = j1;
                                    dDist_BestBox = dDist;
                                    numStickSide_BestBox = numStickSide;
                                    widthBestBox = widthBox; heightBestBox = heightBox; 
                                end
                            end
                        end
                    end
                end
        end
        % ����ÿ��ռ�ǵ������ģ�飨��Ѩ����С��
        HoleDensity(k1,1) = dmin_Best; 
        HoleDensity(k1,2) = IndexBestBox;
        HoleDensity(k1,3) = widthBestBox; 
        HoleDensity(k1,4) = heightBestBox; 
        HoleDensity(k1,5) = dDist_BestBox;
        HoleDensity(k1,6) = numStickSide_BestBox;
    end

    % ��������ռ��
    IndexAngle = 0; dmin_Best = 100; 
    for k1 = 1:numAngle
        if HoleDensity(k1,1)<dmin_Best
            dmin_Best = HoleDensity(k1,1);
            Index_BestBox = HoleDensity(k1,2);
            Area_BestBox = HoleDensity(k1,3)*HoleDensity(k1,4);
            IndexAngle = k1;
        elseif abs(HoleDensity(k1,1) - dmin_Best)< 1e-4
            if HoleDensity(k1,3)*HoleDensity(k1,4)> Area_BestBox
                dmin_Best = HoleDensity(k1,1);
                Index_BestBox = HoleDensity(k1,2);
                Area_BestBox = HoleDensity(k1,3)*HoleDensity(k1,4);
                IndexAngle = k1;
            end
        end
    end
    
    % ����ģ���δ��伯����������伯��
    if IndexAngle<0.1 || Index_BestBox<0.5
        break;
    end
    unFinishBox(Index_BestBox) = 0;
    numFinishedBox = numFinishedBox + 1;
    FinishedBox(numFinishedBox,1) = Index_BestBox;
    y0 = AnglePara(IndexAngle,1); x0 = AnglePara(IndexAngle,2); 
    widthBestBox = HoleDensity(IndexAngle,3);
    heightBestBox = HoleDensity(IndexAngle,4);
    nAngleType = AnglePara(IndexAngle,3);
    % ��¼��ģ������Ͻ�����
    switch nAngleType
        case 1
            FinishedBox(numFinishedBox,2) = x0;
            FinishedBox(numFinishedBox,3) = y0 - heightBestBox + 1;            
        case 2
            FinishedBox(numFinishedBox,2) = x0 - widthBestBox + 1;
            FinishedBox(numFinishedBox,3) = y0 - heightBestBox + 1;
        case 3
            FinishedBox(numFinishedBox,2) = x0 - widthBestBox + 1;
            FinishedBox(numFinishedBox,3) = y0;
        case 4
            FinishedBox(numFinishedBox,2) = x0;
            FinishedBox(numFinishedBox,3) = y0;
    end
    FinishedBox(numFinishedBox,4) = widthBestBox; % ��¼��ģ��Ŀ��
    FinishedBox(numFinishedBox,5) = heightBestBox; % ��¼��ģ��ĸ߶�

    % �����Ͻ�Ϊ��׼���þ��ο鲢��ע�߽�
    YIndexTop = FinishedBox(numFinishedBox,3);
    YIndexBottom = FinishedBox(numFinishedBox,3) + heightBestBox - 1;    
    XIndexLeft = FinishedBox(numFinishedBox,2);
    XIndexRight = FinishedBox(numFinishedBox,2) + widthBestBox - 1;
    C(YIndexTop:YIndexBottom,XIndexLeft:XIndexRight) = 1;
    C(YIndexTop,XIndexLeft:XIndexRight) = 2;   C(YIndexBottom,XIndexLeft:XIndexRight) = 2;
    C(YIndexTop:YIndexBottom,XIndexLeft) = 2;  C(YIndexTop:YIndexBottom,XIndexRight) = 2;   
end
% ����δ������ľ��ο�
unFinishBoxNo = [];
for j=1:numBox
    j1 = unFinishBox(j);
    if j1>0
        unFinishBoxNo = [unFinishBoxNo j1];
    end
end
% �������������
sumArea = 0;
for  k=1:numFinishedBox
    sumArea = sumArea + FinishedBox(k,4)*FinishedBox(k,5);
end
dAreaRatio = sumArea/(WBox*HBox)*100;

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
function [dDist, numStickSide] = ShadowDistance(widthBox, heightBox, x0, y0, nAngleType, C, WBox, HBox)
% ���ںϷ�ռ�ǲ���,���������������С�����پ���
switch nAngleType
    case 1
        YIndex = (y0 - heightBox+1):y0;
        XIndex = x0:(x0 + widthBox - 1);
        % �ȼ����ȷ����ͶӰ
        for j = x0:(WBox+2)
            wShadow(j-x0+1) = sum(C(YIndex,j));
        end
        for i1 = 1:length(wShadow);
            if wShadow(i1)>0.5
                xDist = i1 - 1;     break;
            end
        end
        xDist = xDist - widthBox;
        % �ټ���߶ȷ����ͶӰ
        for j = 1:y0
            hShadow(j) = sum(C(j,XIndex));
        end
        for i2 = 1:y0;
            if hShadow(y0 -i2+1)>0.5
                yDist = i2 - 1;     break;
            end
        end
        yDist = yDist - heightBox;
        
    case 2
        YIndex = (y0 - heightBox+1):y0;
        XIndex = (x0 - widthBox+1):x0;
        % �ȼ����ȷ����ͶӰ
        for j = 1:x0
            wShadow(j) = sum(C(YIndex,j));
        end
        for i1 = 1: x0;
            if wShadow(x0-i1+1)>0.5
                xDist = i1 - 1;  break;
            end
        end
        xDist = xDist - widthBox;
        % �ټ���߶ȷ����ͶӰ
        for j = 1:y0
            hShadow(j) = sum(C(j,XIndex));
        end
        for i2 = 1:y0;
            if hShadow(y0-i2+1)>0.5
                yDist = i2 - 1;  break;
            end
        end
        yDist = yDist - heightBox;
        
    case 3
        YIndex = y0:(y0+heightBox-1);
        XIndex = (x0-widthBox+1):x0;
        % �ȼ����ȷ����ͶӰ
        for j = 1:x0
            wShadow(j) = sum(C(YIndex,j));
        end
        for i1 = 1: x0;
            if wShadow(x0-i1+1)>0.5
                xDist = i1 - 1;  break;
            end
        end
        xDist = xDist - widthBox;
        % �ټ���߶ȷ����ͶӰ
        for j = y0:(HBox+2)
            hShadow(j-y0+1) = sum(C(j,XIndex));
        end
        for i2 = 1:length(hShadow);
            if hShadow(i2)>0.5
                yDist = i2 - 1;  break;
            end
        end
        yDist = yDist - heightBox;

    case 4
        YIndex = y0:(y0+heightBox-1);
        XIndex = x0:(x0+widthBox-1);
        % �ȼ����ȷ����ͶӰ
        for j = x0:(WBox+2)
            wShadow(j-x0+1) = sum(C(YIndex,j));
        end
        for i1 = 1:length(wShadow);
            if wShadow(i1)>0.5
                xDist = i1 - 1;     break;
            end
        end
        xDist = xDist - widthBox;
        % �ټ���߶ȷ����ͶӰ
        for j = y0:(HBox+2)
            hShadow(j-y0+1) = sum(C(j,XIndex));
        end
        for i2 = 1:length(hShadow);
            if hShadow(i2)>0.5
                yDist = i2 - 1;  break;
            end
        end
        yDist = yDist - heightBox;
end
numStickSide = 2;
if xDist<=0.1,
    xDist = 0;
    numStickSide = numStickSide + 1;
end
if yDist<=0.1,  
    yDist = 0;  
    numStickSide = numStickSide + 1;
end
dDist = min(xDist, yDist);


        



