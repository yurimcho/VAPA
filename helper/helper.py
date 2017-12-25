
def LinerarInterpolation(x, x0, x1, y0, y1):
    """1D 선형 보간
    Parameters
    ----------
    x : 현재 값
    x0 : 현재 값에 해당하는 차원의 시작 값
    x1 : 현재 값에 해당하는 차원의 끝 값
    y0 : 알고자 하는 차원의 시작 값
    y1 : 알고자 하는 차원의 끝 값
    Returns
    -------
    result : float value
        x 값에 해당하는 y 차원의 값
    """
    return y0 + (x - x0) * (y1 - y0) / (x1 - x0)

