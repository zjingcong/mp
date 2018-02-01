import numpy as np
from math import sqrt


class Agent(object):
    def __init__(self, csvParameters, ksi=0.5, dhor = 10, timehor=5, goalRadiusSq=1, maxF = 10):
        """ 
            Takes an input line from the csv file,  
            and initializes the agent
        """
        self.id = int(csvParameters[0]) # the id of the agent
        self.gid = int(csvParameters[1]) # the group id of the agent
        self.pos = np.array([float(csvParameters[2]), float(csvParameters[3])]) # the position of the agent 
        self.vel = np.zeros(2) # the velocity of the agent
        self.goal = np.array([float(csvParameters[4]), float(csvParameters[5])]) # the goal of the agent
        self.prefspeed = float(csvParameters[6]) # the preferred speed of the agent
        self.gvel = self.goal-self.pos # the goal velocity of the agent
        self.gvel = self.gvel/(sqrt(self.gvel.dot(self.gvel )))*self.prefspeed       
        self.maxspeed = float(csvParameters[7]) # the maximum sped of the agent
        self.radius = float(csvParameters[8]) # the radius of the agent
        self.goalRadiusSq =goalRadiusSq # parameter to determine if agent is close to the goal
        self.atGoal = False # has the agent reached its goal?
        self.ksi = ksi # the relaxation time used to compute the goal force
        self.dhor = dhor # the sensing radius
        self.timehor = timehor # the time horizon for computing avoidance forces
        self.F = np.zeros(2) # the total force acting on the agent
        self.maxF = maxF # the maximum force that can be applied to the agent

    # compute time to collide
    def _ttc(self, agent):
        r = self.radius + agent.radius
        x = self.pos - agent.pos
        c = np.dot(x, x) - r * r
        if c < 0:   # agents are colliding
            return 0
        v = self.vel - agent.vel
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

    def computeForces(self, neighbors=[]):
        """ 
            Your code to compute the forces acting on the agent. 
            You probably need to pass here a list of all the agents in the simulation to determine the agent's nearest neighbors
        """
        if not self.atGoal:
            # goal force
            self.F = (self.gvel - self.vel) / self.ksi

            # avoidance force
            Favoid = np.zeros(2)
            for neighbor in neighbors:
                tau = self._ttc(neighbor)
                if tau is np.inf:
                    favoid = np.zeros(2)
                else:
                    dir = self.pos + self.vel * tau - neighbor.pos - neighbor.vel * tau
                    if tau is 0:
                        tau = np.finfo('float64').eps
                    dir_norm = np.linalg.norm(dir, ord=1)
                    if dir_norm is 0:
                        dir_norm = np.finfo(dir.dtype).eps
                    n = dir / dir_norm
                    favoid = (max(self.timehor - tau, 0) / tau) * n
                Favoid = Favoid + favoid
            self.F = self.F + Favoid

            # force refinement
            F = np.linalg.norm(self.F, ord=1)
            if F is 0:
                F = np.finfo(self.F.dtype).eps
            if F > self.maxF:
                self.F = self.maxF * (self.F / F)

    def update(self, dt):
        """ 
            Code to update the velocity and position of the agents.  
            as well as determine the new goal velocity 
        """
        if not self.atGoal:
            self.vel += self.F*dt     # update the velocity
            self.pos += self.vel*dt   # update the position
        
            # compute the goal velocity for the next time step. do not modify this
            self.gvel = self.goal - self.pos
            distGoalSq = self.gvel.dot(self.gvel)
            if distGoalSq < self.goalRadiusSq: 
                self.atGoal = True  # goal has been reached
            else: 
                self.gvel = self.gvel/sqrt(distGoalSq)*self.prefspeed
