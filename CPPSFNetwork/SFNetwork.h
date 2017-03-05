#pragma once
#include "Edge.h"
#include "RK4S.h"
#include "ilRand.h"

#include <vector>
#include <fstream>




class Edge
{
public:
	Edge();
	Edge(int, int, int);
	~Edge();

	int from = -1,
		to = -1,
		weight = 0;

	void Binarize(std::ofstream& str);
};




class SFNetwork
{
public:
	SFNetwork(int node_count, int mlt);
	SFNetwork(int node_count, int mlt, int seed);
	~SFNetwork();

	int node_count = 0,
		multiplier = 1,
		seed;
	std::vector<Edge> edges;
private:
	void generate();
protected:
	ilRand ilRandom;
};

 



class SFNetworkOscillatorState {
public:
	double time;
	std::vector<double> phases;

	SFNetworkOscillatorState(double, std::vector<double>);
	~SFNetworkOscillatorState();

	void Binarize(std::ofstream& str);
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

	std::vector<double> phases,
		freqs;
	std::vector<SFNetworkOscillatorState> states;

	SFNetworkOscillator(int node_count, int mlt, double strength, double freq_min, double freq_max, double phase_min, double phase_max, double time_init, double time_step, double solve_step);
	SFNetworkOscillator(int node_count, int mlt, double strength, double freq_min, double freq_max, double phase_min, double phase_max, double time_init, double time_step, double solve_step, int random_seed);
	~SFNetworkOscillator();

	void SimulateDynamicStep();

	void Binarize(const char* path, char version);
private:
	std::vector<RK4SFunc> funcs;

	void constructor(double str, double f_min, double f_max, double p_min, double p_max, double t_init, double t_step, double s_step);
	void phasesNormalize();

	void Binarize_v1(const char* path);
};