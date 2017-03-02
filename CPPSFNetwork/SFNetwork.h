#pragma once
#include "Edge.h"
#include "RK4S.h"

#include <vector>




class Edge
{
public:
	Edge();
	Edge(int, int, int);
	~Edge();

	int from = -1,
		to = -1,
		weight = 0;
};




class SFNetwork
{
public:
	SFNetwork(int, int);
	~SFNetwork();

	int node_count = 0,
		multiplier = 1;
	std::vector<Edge> edges;
protected:
	int random(int, int);
	double random(double, double);
};

 



class SFNetworkOscillatorState {
public:
	double time;
	std::vector<double> phases;

	SFNetworkOscillatorState(double, std::vector<double>);
	~SFNetworkOscillatorState();
};


class SFNetworkOscillator : SFNetwork {
public:
	double strength,
		freq_init_min,
		freq_init_max,
		phase_init_min,
		phase_init_max,
		time_init,
		time_step,
		solve_step,
		
		time;
	int random_seed;

	std::vector<double> phases,
		freqs;
	std::vector<SFNetworkOscillatorState> states;

	SFNetworkOscillator(int node_count, int mlt, double strength, double freq_min, double freq_max, double phase_min, double phase_max, double time_init, double time_step, double solve_step);
	SFNetworkOscillator(int node_count, int mlt, double strength, double freq_min, double freq_max, double phase_min, double phase_max, double time_init, double time_step, double solve_step, int random_seed);
	~SFNetworkOscillator();

	void SimulateDynamicStep();
private:
	std::vector<RK4SFunc> funcs;

	void constructor(double str, double f_min, double f_max, double p_min, double p_max, double t_init, double t_step, double s_step, int seed);
	void phasesNormalize();
};