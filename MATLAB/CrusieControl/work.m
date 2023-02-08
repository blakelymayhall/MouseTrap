% Model Coeffs
m = 1000;
b = 50;

% Build SS Model
A = -b/m;
B = 1/m;
C = 1;
D = 0;
Plant = ss(A,B,C,D);

% Set gains
Kp = 300;
Kd = 20;
Ki = 15;

% Test Controller, want the following:
%{
Rise time < 5 s
Overshoot < 10%
Steady-state error < 2%
%}
s = tf('s');
Cont = Kp + Ki/s + Kd*s;

% Model Closed-Loop System
sys = Cont*Plant/(1+Cont*Plant);
stepinfo(sys)

% Set external input
% open_system('cruise_control_sys');
t = (0:0.01:600)';
set_point = [t,15*ones(size(t))];

% Run sim
[t_out,x_out,u] = sim('cruise_control_sys',50);

% Plot velocity
plot(t_out, u)

% Final SS Error
ss_err = abs((15-u(size(u,1))))/15*100