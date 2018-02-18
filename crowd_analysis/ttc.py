import numpy as np
from math import *


# compute time to collide
def ttc(curr_agent, agent):
    curr_agent_pos = np.full(2, [curr_agent.get('x'), curr_agent.get('y')])
    curr_agent_vel = np.full(2, [curr_agent.get('velocity_x'), curr_agent.get('velocity_y')])
    agent_pos = np.full(2, [agent.get('x'), agent.get('y')])
    agent_vel = np.full(2, [agent.get('velocity_x'), agent.get('velocity_y')])
    
    r = curr_agent.get('radius') + agent.get('radius')
    x = curr_agent_pos - agent_pos
    c = np.dot(x, x) - r * r
    if c < 0:   # agents are colliding
        return 0
    v = curr_agent_vel - agent_vel
    a = np.dot(v, v)
    b = np.dot(x, v)
    if b > 0:   # agents are moving away
        return np.inf
    discr = b * b - a * c
    if discr <= 0:
        return np.inf
    tau = c / (-b + sqrt(discr))
    if tau < 0:
        return np.inf
    return tau
