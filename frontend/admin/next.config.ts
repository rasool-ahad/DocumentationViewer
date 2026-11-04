import type { NextConfig } from "next";
import { env } from "./src/config/env";

const nextConfig: NextConfig = {
  agentRules: false,
  async rewrites() {
    return [
      {
        source: "/backend/:path*",
        destination: `${env.apiBaseUrl}/api/:path*`,
      },
    ];
  },
};

export default nextConfig;
