#pragma once
#include "ilRand.h"
#include "RK4S.h"

#include <vector>
#include <fstream>

#define ulong unsigned long long

namespace Diploma2 {
	

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
		static Edge Debinarize(std::ifstream& str);
	};


	class SFNetwork
	{
	public:
		SFNetwork();
		SFNetwork(int node_count, int mlt);
		SFNetwork(int node_count, int mlt, int seed);
		SFNetwork(int node_count, int mlt, std::vector<Edge> &edgs, ilRand &rnd);
		~SFNetwork();

		void initialize_sfnw(int node_count, int mlt);
		void initialize_sfnw(int node_count, int mlt, int seed);
		void initialize_sfnw(int node_count, int mlt, std::vector<Edge> &edgs, ilRand &rnd);

		int node_count = 0,
			multiplier = 1,
			seed;
		std::vector<Edge> edges;
	private:
		void generate();
	protected:
		ilRand ilRandom;
	};

}