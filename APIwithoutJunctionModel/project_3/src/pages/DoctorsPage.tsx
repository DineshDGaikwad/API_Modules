import { useEffect, useState } from "react";
import { api } from "../services/api";
import type { GetDocDTO } from "../types/GetDocDto";

export const DoctorsPage = () => {
  const [lstdoctors, setDoctors] = useState<GetDocDTO[]>([]);

  useEffect(() => {
    api
      .get<GetDocDTO[]>("/Doctors")
      .then((res) => setDoctors(res.data))
      .catch((err) => console.error("API error", err));
  }, []);

  const styles = {
    container: {
      padding: "20px",
      fontFamily: "Arial, sans-serif",
    },
    heading: {
      marginBottom: "20px",
      fontSize: "48px",
      fontWeight: "600",
      textAlign: "center" as const,
    },
    table: {
      width: "100%",
      borderCollapse: "collapse" as const,
      marginTop: "20px",
      backgroundColor: "#fff",
      color: "#ff0303ff",
      boxShadow: "0 2px 8px rgba(0,0,0,0.1)",
      borderRadius: "25px",
      overflow: "hidden",
    },
    th: {
      backgroundColor: "#2f3e46",
      color: "white",
      padding: "14px",
      textAlign: "left" as const,
      fontSize: "20px", 
      fontWeight: 600,
    },
    td: {
      padding: "14px",
      borderBottom: "1px solid #ddd",
      fontSize: "18px", 
    },
    evenRow: {
      backgroundColor: "#f1f3f4",
    },
    oddRow: {
      backgroundColor: "white",
    },
  };

  return (
    <div style={styles.container}>
      <h1 style={styles.heading}>Doctors List</h1>

      <table style={styles.table}>
        <thead>
          <tr>
            <th style={styles.th}>Doctor ID</th>
            <th style={styles.th}>Name</th>
            <th style={styles.th}>Specialty</th>
            <th style={styles.th}>Patients</th>
          </tr>
        </thead>

        <tbody>
          {lstdoctors.map((doc, index) => {
            const rowStyle = index % 2 === 0 ? styles.evenRow : styles.oddRow;

            return (
              <tr
                key={doc.doctorId}
                style={rowStyle}
                onMouseEnter={(e) =>
                  (e.currentTarget.style.backgroundColor = "#e3e6e8")
                }
                onMouseLeave={(e) =>
                  (e.currentTarget.style.backgroundColor =
                    index % 2 === 0 ? "#f1f3f4" : "white")
                }
              >
                <td style={styles.td}>{doc.doctorId}</td>
                <td style={styles.td}>{doc.name}</td>
                <td style={styles.td}>{doc.specialty}</td>
                <td style={styles.td}>
                  {doc.patients?.length
                    ? doc.patients.join(", ")
                    : "No patients"}
                </td>
              </tr>
            );
          })}
        </tbody>
      </table>
    </div>
  );
};
